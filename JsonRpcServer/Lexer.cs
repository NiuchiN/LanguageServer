﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace JsonRpcServer
{
    public class Token : IComparable<Token>
    {
        public int Line { get; set; }
        public int StartChar { get; set; }
        public int TokenType { get; set; }
        public int TokenModifiers { get; set; }
        public string TokenString { get; set; }

        public int CompareTo(Token t2)
        {
            if (this.Line < t2.Line) {
                return -1;
            }
            else if (this.Line > t2.Line) {
                return 1;
            }
            else {
                if (this.StartChar < t2.StartChar) {
                    return -1;
                }
                else if (this.StartChar > t2.StartChar) {
                    return 1;
                }
                else {
                    return 0;
                }
            }
        }
    }

    public class Lexer : IDisposable
    {
        private string _input;
        private string _uri;
        private int _curLine;
        private int _curCharacter;
        private int _curInputPos;
        private List<Token> _lstToken = new List<Token>();

        public Lexer(string uri, string input)
        {
            _uri = uri;
            _input = input;
            _curLine = 0;
            _curCharacter = 0;
            _curInputPos = 0;
        }
        
        private void setCommonTokenInfo(Token token)
        {
            token.Line = _curLine;
            token.StartChar = _curCharacter - 1;
            token.TokenModifiers = 0;
        }

        private void countUpReadPos()
        {
            if (_curInputPos < _input.Length) {
                _curInputPos += 1;
                _curCharacter += 1;
            }
        }

        private void coutUpLine()
        {
            _curLine += 1;
            _curCharacter = 0;
        }

        private char readChar()
        {
            if (_curInputPos >= _input.Length) {
                return '\0';
            } else {
                char ret = _input[_curInputPos];
                return ret;
            }
        }

        private char readNextChar()
        {
            if((_curInputPos + 1) >= _input.Length) {
                return '\0';
            } else {
                return _input[_curInputPos + 1];
            }
        }

        private char skipWhiteSpace()
        {
            char curChar = '\0';
            bool bIsWhiteSpace = true;

            while(bIsWhiteSpace) { 
                curChar = readChar();

                if (curChar == '\0') { return curChar; }

                countUpReadPos();
                bIsWhiteSpace = isWhiteSpace(curChar);
            } 
            return curChar;
        }

        private bool isWhiteSpace(char c)
        {
            return (' ' == c)
                || ('\t' == c)
                || ('\r' == c);
        }

        private bool isLetter(char c)
        {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || c == '_';
        }
        private bool isNumber(char c)
        {
            return ('0' <= c && c <= '9');
        }

        private bool isIdentifier(char c)
        {
            return isLetter(c) || isNumber(c);
        }

        private bool isRegister(string str)
        {
            return (str[0] == 'x') 
                || (str[0] == 'w');
        }

        private string readIdentifier(char beginChar)
        {
            string retStr = beginChar.ToString();
            while(true) {
                char curChar = readChar();
                countUpReadPos();
                if (isIdentifier(curChar)) {
                    retStr += curChar;
                } else {
                    break;
                }
            }

            return retStr;
        }


        private Token readComment()
        {
            char curChar;
            int startPos = _curInputPos - 1;
            Token retToken = new Token();

            setCommonTokenInfo(retToken);
            retToken.TokenType = (int)SemTokensTypeIdx.Comment;

            do {
                curChar = readChar();
                countUpReadPos();

            } while (curChar != '\n' 
                    && curChar != '\0'); 
            coutUpLine();

            retToken.TokenString = _input.Substring(startPos, (_curInputPos - startPos));
            return retToken;
        }

        private bool isOtherKeyword(Token token)
        {
            foreach (OtherKeywordType key in Enum.GetValues(typeof(OtherKeywordType))) {
                if (token.TokenString == key.GetStr()) {
                    return true;
                }
            }
            return false;
        }

        private Token readStartKeyword(char beginChar)
        {
            Token retToken = new Token();
            setCommonTokenInfo(retToken);

            string strVal = readIdentifier(beginChar);
            foreach (PeriodKeywordType key in Enum.GetValues(typeof(PeriodKeywordType))) {
                if (key == PeriodKeywordType.Banpei) {
                    return null;
                }
                if (strVal == key.GetStr()) {
                    retToken.TokenType = (int)SemTokensTypeIdx.Keyword;
                    retToken.TokenString = strVal;
                    break;
                }
            }

            return retToken;
        }

        private Token readStringLiteral()
        {
            Token retToken = new Token();
            bool bIsStringLiteral = false;
            bool bIsContinue = true;
            string strValToken = "'";

            setCommonTokenInfo(retToken);

            while (bIsContinue) {
                char curChar = readChar();
                countUpReadPos();

                switch (curChar) {
                    case '\'':
                        strValToken += curChar;
                        bIsStringLiteral = true;
                        bIsContinue = false;
                        break;
                    case '\n':
                        coutUpLine();
                        bIsContinue = false;
                        break;
                    case '\r':
                    case '\0':
                        bIsContinue = false;
                        break;
                    default:
                        strValToken += curChar;
                        break;
                }
            }

            if (bIsStringLiteral) {
                Token prevToken = new Token();

                retToken.TokenString = strValToken;

                if (_lstToken.Count > 0) {
                    prevToken = _lstToken[_lstToken.Count - 1];
                    if (prevToken.TokenString == PeriodKeywordType.BeginBlock.GetStr() || prevToken.TokenString == PeriodKeywordType.BeginTemplate.GetStr() ) {
                        retToken.TokenType = (int)SemTokensTypeIdx.Method;
                    } else if (prevToken.TokenString == OtherKeywordType.Macro.GetStr() || prevToken.TokenString == PeriodKeywordType.CallMacro.GetStr()) {
                        retToken.TokenType = (int)SemTokensTypeIdx.Macro;
                    } else if (prevToken.TokenString == PeriodKeywordType.BeginFunction.GetStr()) {
                        retToken.TokenType = (int)SemTokensTypeIdx.Function;
                    } else if (prevToken.TokenString == PeriodKeywordType.Define.GetStr()) {
                        retToken.TokenType = (int)SemTokensTypeIdx.Parameter;
                    } else {
                        retToken.TokenType = (int)SemTokensTypeIdx.String;
                    }
                } else {
                    retToken.TokenType = (int)SemTokensTypeIdx.String;
                }

            } else {
                retToken = null;
            }

            return retToken;
        }

        private Token readNumberLiteral()
        {
            Token retToken = new Token();
            bool bIsLiteral = false;
            bool bIsContinue = true;
            string strValToken = "#";
            setCommonTokenInfo(retToken);

            while (bIsContinue) {
                char curChar = readChar();
                countUpReadPos();

                switch (curChar) {
                    case '+':
                    case '-':
                        if (strValToken.Length == 1) {
                            // +と-の記号が有効なのは、2文字目の時だけ。
                            strValToken += curChar;
                        } 
                        break;
                    case '\n':
                        coutUpLine();
                        bIsContinue = false;
                        break;
                    case '\r':
                    case '\0':
                        bIsContinue = false;
                        break;
                    default:
                        if (isNumber(curChar)) {
                            bIsLiteral = true;
                            strValToken += curChar;
                        } else if (isWhiteSpace(curChar)) {
                            bIsContinue = false;
                        } else {
                            bIsLiteral = true;
                            strValToken += curChar;
                        }
                        break;
                }
            }

            if (bIsLiteral) {
                retToken.TokenString = strValToken;
                retToken.TokenType = (int)SemTokensTypeIdx.Number;
            } else {
                retToken = null;
            }
                return retToken;
        }

        private Token readOneToken(InstructionHashList instHashList, out bool bIsEOF)
        {
            char curChar;
            Token retToken = new Token();
            bool bIsContinue = true;
            bIsEOF = false;

            while (bIsContinue) {
                curChar = skipWhiteSpace();
                switch (curChar) {
                    case '\n':
                        coutUpLine();
                        break;
                    case ';':
                        retToken = readComment();
                        bIsContinue = false;
                        break;
                    case '.':
                        retToken = readStartKeyword('.');
                        bIsContinue = false;
                        break;
                    case '\'':
                        retToken = readStringLiteral();
                        if (retToken != null) {
                            bIsContinue = false;
                        }
                        break;
                    case '#':
                        retToken = readNumberLiteral();
                        if (retToken != null) {
                            bIsContinue = false;
                        }
                        break;
                    case '\0':
                        retToken = null;
                        bIsEOF = true;
                        bIsContinue = false;
                        break;
                    default:
                         if (isIdentifier(curChar)) {
                             setCommonTokenInfo(retToken);
                             retToken.TokenString = readIdentifier(curChar);

                             if (isRegister(retToken.TokenString)) {
                                 retToken.TokenType = (int)SemTokensTypeIdx.Variable;
                             } else if (instHashList.IsExistInstruction(retToken.TokenString)) {
                                 retToken.TokenType = (int)SemTokensTypeIdx.Instruction;
                             } else if (isOtherKeyword(retToken)) {
                                 retToken.TokenType = (int)SemTokensTypeIdx.Keyword;
                             } else {
                                 retToken = null;
                             }
                             bIsContinue = false;
                         } else {
                             // pass;
                         }
                        break;
                }
            }
            return retToken;
        }

        public void Tokenize(InstructionHashList instHashList)
        {
            Token token = new Token();
            while (true) {
                bool bIsEOF;
                token = readOneToken(instHashList, out bIsEOF);
                if (bIsEOF) break;

                if (token != null) {
                    _lstToken.Add(token);
                }
                //Console.Error.WriteLine($"Token:{(SemTokensTypeIdx)token.TokenType} Line:{token.Line} StartChar:{token.StartChar}, Length:{token.TokenString.Length}, string:{token.TokenString}");
            }
        }

        public void InitToken(string text)
        {
            _input = text;
            _curLine = 0;
            _curCharacter = 0;
            _curInputPos = 0;
            _lstToken.Clear();
        }

        public void MakeResult(List<int> lstResult)
        {
            int iCurLine = 0;
            int iCurCharcter = 0;

            foreach (Token token in _lstToken) {
                int[] aiResultOne = new int[5];
                int iDeltaLine, iDeltaStart, iLength, iTokenType, iTokenModifiers;
                if (token.Line == iCurLine) {
                    iDeltaLine = 0;
                    iDeltaStart = token.StartChar - iCurCharcter;

                } else {
                    iDeltaLine = token.Line - iCurLine;
                    iDeltaStart = token.StartChar;
                }

                iCurLine = token.Line;
                iCurCharcter = token.StartChar;
                iLength = token.TokenString.Length;
                iTokenType = token.TokenType;
                iTokenModifiers = token.TokenModifiers;

                aiResultOne[0] = iDeltaLine;
                aiResultOne[1] = iDeltaStart;
                aiResultOne[2] = iLength;
                aiResultOne[3] = iTokenType;
                aiResultOne[4] = iTokenModifiers;
                foreach (int res in aiResultOne) {
                    lstResult.Add(res);
                }
            }
        }

        public Token SearchToken(int line, int startChar)
        {
            Token token = new Token();
            token.Line = line;
            token.StartChar = startChar;

            var index = _lstToken.BinarySearch(token);

            if (index >= 0) {
                return _lstToken[index];
            } else {
                index = ~index;
                return _lstToken[index - 1];
            }
        }
        public void Dispose()
        {
            _uri = null;
            _input = null;
            _curLine = 0;
            _curCharacter = 0;
            _curInputPos = 0;
            _lstToken.Clear();
        }

    }

    public class StringValAttribute : Attribute
    {
        public string StringVal { get; protected set; }
        public StringValAttribute(string strVal) { this.StringVal = strVal; }
    }

    public static class CommonAttr
    {
        public static string GetStr(this  Enum eVal)
        {
            Type type = eVal.GetType();
            System.Reflection.FieldInfo fieldInfo = type.GetField(eVal.ToString());

            if (fieldInfo == null ) {
                return null;
            }

            StringValAttribute[] attr = fieldInfo.GetCustomAttributes(typeof(StringValAttribute), false) as StringValAttribute[];
            return attr.Length > 0 ? attr[0].StringVal : null;
        }
    }

    public enum SemTokensTypeIdx
    {
        [StringVal("namespace")]
        NameSpace = 0,

        [StringVal("class")]
        Class = 1,

        [StringVal("enum")]
        Enum = 2,

        [StringVal("interface")]
        Interface = 3,

        [StringVal("struct")]
        Struct = 4,

        [StringVal("typeParameter")]
        TypeParameter = 5,

        [StringVal("type")]
        Type = 6,

        [StringVal("parameter")]
        Parameter = 7,

        [StringVal("variable")]
        Variable = 8,

        [StringVal("property")]
        Property = 9,

        [StringVal("enumMember")]
        EnumMember = 10,

        [StringVal("decorator")]
        Decorator = 11,

        [StringVal("event")]
        Event = 12,

        [StringVal("function")]
        Function = 13,

        [StringVal("method")]
        Method = 14,

        [StringVal("macro")]
        Macro = 15,

        [StringVal("label")]
        Lable = 16,

        [StringVal("comment")]
        Comment = 17,

        [StringVal("string")]
        String = 18,

        [StringVal("keyword")]
        Keyword = 19,

        [StringVal("number")]
        Number = 20,

        [StringVal("regexp")]
        Regexp = 21,

        [StringVal("operator")]
        Operator = 22,

        [StringVal("modifier")]
        Modifier = 23,

        [StringVal("instruction")] // オリジナルのTokenType
        Instruction = 24,

        MaxNum,
    }

    public enum SemTokensModifyIdx
    {
        [StringVal("declaration")]
        Declaration = 0,

        [StringVal("definition")]
        Definition = 1,

        [StringVal("readonly")]
        Readonly = 2,

        [StringVal("static")]
        Static = 3,

        [StringVal("deprecated")]
        Deprecated = 4,

        [StringVal("abstract")]
        Abstract = 5,

        [StringVal("async")]
        Async = 6,

        [StringVal("modification")]
        Modification = 7,

        [StringVal("documentation")]
        Documentation = 8,

        [StringVal("defaultLibrary")]
        DefaultLibrary = 9,

        MaxNum,
    }


}
