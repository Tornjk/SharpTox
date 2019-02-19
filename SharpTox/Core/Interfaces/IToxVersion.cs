namespace SharpTox.Core.Interface
{
    public interface IToxVersion
    {
        uint Major { get; }
        uint Minor { get; }
        uint Patch { get; }

        bool IsCompatible();
    }
}