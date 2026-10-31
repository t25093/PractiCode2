using System.Text.RegularExpressions;

public class HtmlSerializer
{
    public async Task<string> Load(string url)
    {
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        var html = await response.Content.ReadAsStringAsync();
        return html;
    }

    private List<string> SplitToParts(string html)
    {
        var cleanHtml = Regex.Replace(html, @"\s+", " ");

        var parts = Regex.Split(cleanHtml, @"(<[^>]*>)")
                         .Select(s => s.Trim())
                         .Where(s => s.Length > 0)
                         .ToList();
        return parts;
    }

    public HtmlElement Serialize(string html)
    {

    }
}
