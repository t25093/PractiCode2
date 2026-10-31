using System.Net.Http;
using System.Text.RegularExpressions;

namespace HtmlSerializerProject
{
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
            List<string> parts = SplitToParts(html);

            var root = new HtmlElement { Name = "root" };
            var current = root;

            foreach (var part in parts)
            {
                if (!part.StartsWith("<"))
                {
                    current.InnerHtml += part;
                    continue;
                }

                var content = part.Substring(1, part.Length - 2).Trim();

                if (content.Length == 0 || content.StartsWith("!"))
                    continue;

                var firstWord = content.Split(' ')[0].ToLower();

                if (firstWord == "/html")
                    break;

                if (firstWord.StartsWith("/"))
                {
                    if (current.Parent != null)
                        current = current.Parent;
                    continue;
                }

                firstWord = firstWord.TrimEnd('/');

                if (!HtmlHelper.Instance.HtmlTags.Contains(firstWord))
                    continue;

                var element = new HtmlElement
                {
                    Name = firstWord,
                    Parent = current
                };
                current.Children.Add(element);

                var attributesString = content.Substring(firstWord.Length);
                var attrMatches = Regex.Matches(attributesString,
                    @"([^\s=/]+)\s*=\s*(""[^""]*""|'[^']*')");

                foreach (Match match in attrMatches)
                {
                    var attrName = match.Groups[1].Value.ToLower();
                    var attrValue = match.Groups[2].Value.Trim('"', '\'');

                    element.Attributes[attrName] = attrValue;

                    if (attrName == "class")
                        element.Classes = attrValue
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                    else if (attrName == "id")
                        element.Id = attrValue;
                }

                bool isSelfClosing = content.EndsWith("/") ||
                                     HtmlHelper.Instance.HtmlVoidTags.Contains(firstWord);

                if (!isSelfClosing)
                    current = element;

            }
            return root;
        }
    }
}