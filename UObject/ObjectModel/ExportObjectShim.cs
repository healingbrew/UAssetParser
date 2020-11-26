namespace UObject.ObjectModel
{
    public class ExportObjectShim : AbstractExportObject
    {
        public string? ExpectedClass { get; set; }
        public int    EndOffset     { get; set; }
    }
}
