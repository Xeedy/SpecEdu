using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SpecEdu.Web.Public.TagHelpers;

/// <summary>
/// Renders an icon from wwwroot/img/icons/{name}.svg as an inline SVG.
/// Usage: &lt;spec-icon name="folder" size="32" class="text-spec-primary"&gt;&lt;/spec-icon&gt;
/// </summary>
[HtmlTargetElement("spec-icon", TagStructure = TagStructure.NormalOrSelfClosing)]
public class IconTagHelper : TagHelper
{
    private static readonly ConcurrentDictionary<string, string> _cache = new();
    private static readonly Regex _svgOpen = new(@"<svg\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex _svgClose = new(@"</svg\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly IWebHostEnvironment _env;

    public IconTagHelper(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HtmlAttributeName("name")]
    public string Name { get; set; } = string.Empty;

    [HtmlAttributeName("size")]
    public int Size { get; set; } = 24;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            output.SuppressOutput();
            return;
        }

        var body = _cache.GetOrAdd(Name, key =>
        {
            var safe = Path.GetFileName(key);
            var path = Path.Combine(_env.WebRootPath, "img", "icons", safe + ".svg");
            return File.Exists(path) ? ExtractInner(File.ReadAllText(path)) : string.Empty;
        });

        if (string.IsNullOrEmpty(body))
        {
            output.SuppressOutput();
            return;
        }

        output.TagName = "svg";
        output.TagMode = TagMode.StartTagAndEndTag;

        if (!output.Attributes.ContainsName("xmlns"))
            output.Attributes.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
        if (!output.Attributes.ContainsName("viewBox"))
            output.Attributes.SetAttribute("viewBox", "0 0 16 16");
        if (!output.Attributes.ContainsName("width"))
            output.Attributes.SetAttribute("width", Size.ToString());
        if (!output.Attributes.ContainsName("height"))
            output.Attributes.SetAttribute("height", Size.ToString());
        if (!output.Attributes.ContainsName("fill"))
            output.Attributes.SetAttribute("fill", "currentColor");
        if (!output.Attributes.ContainsName("aria-hidden") && !output.Attributes.ContainsName("role"))
            output.Attributes.SetAttribute("aria-hidden", "true");

        output.Content.SetHtmlContent(body);
    }

    private static string ExtractInner(string svgContent)
    {
        var open = _svgOpen.Match(svgContent);
        var close = _svgClose.Match(svgContent);
        if (!open.Success || !close.Success || close.Index <= open.Index + open.Length)
            return svgContent.Trim();

        var start = open.Index + open.Length;
        var length = close.Index - start;
        return svgContent.Substring(start, length).Trim();
    }
}
