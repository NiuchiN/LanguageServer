namespace Shared
{
    public enum PeriodKeywordType
    {
        [StringVal(".")]
        Begin,

        [StringVal(".")]
        End,

        [StringVal(".")]
        BeginBlock,

        [StringVal(".")]
        EndBlock,

        [StringVal(".")]
        BeginTemplate,

        [StringVal(".")]
        EndTemplate,

        [StringVal(".")]
        BeginFunction,

        [StringVal(".")]
        EndFunction,

        [StringVal(".")]
        CallMacro,


        [StringVal(".")]
        Define,

        [StringVal(".")]
        Tag,

        Banpei
    }

    public enum OtherKeywordType
    {
        [StringVal("")]
        Macro,

        Banpei
    }



}