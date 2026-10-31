public class HtmlElement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
    public List<string> Classes { get; set; } = new();
    public string InnerHtml { get; set; } = "";

    public HtmlElement Parent { get; set; }
    public List<HtmlElement> Children { get; set; } = new();
}
