using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Useful extensions for scraping website data.
/// </summary>
public static partial class ScrapiExtensions
{
#if !NET7_0_OR_GREATER
  private static readonly Regex NumbersOnlyRegex = new(@"([^\d\.])*", RegexOptions.None, TimeSpan.FromSeconds(1));
  private static readonly Regex NoScriptRegex = new(@"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
#endif

  /// <summary>
  /// Remove all non digit characters from the text.
  /// </summary>
  /// <param name="text">The text to extract numbers from.</param>
  /// <param name="includeDecimalPoints">Whether decimal points should be included or removed from the resulting string.</param>
  /// <param name="trim">Trim the resulting string as part of the operation.</param>
  /// <returns>
  /// A string containing only the numbers found in the text.
  /// </returns>
  public static string NumbersOnly(this string? text, bool includeDecimalPoints = false, bool trim = true)
  {
    if (string.IsNullOrEmpty(text))
    {
      return string.Empty;
    }

#if NET7_0_OR_GREATER
    var result = NumbersOnlyRegex().Replace(text, string.Empty);
#else
    var result = NumbersOnlyRegex.Replace(text, string.Empty);
#endif

    if (!includeDecimalPoints)
    {
      result = result.Replace(".", string.Empty);
    }

    if (trim)
    {
      return result.Trim();
    }

    return result;
  }

  /// <summary>
  /// Remove all script tags from HTML.
  /// This is useful if you want to render the resulting HTML without executing any of the script on the page.
  /// </summary>
  /// <param name="html">The HTML to remove script tags from.</param>
  /// <returns>HTML with all script tags removed.</returns>
  public static string HtmlWithNoScript(this string? html)
  {
    if (string.IsNullOrEmpty(html))
    {
      return string.Empty;
    }

#if NET7_0_OR_GREATER
    var result = NoScriptRegex().Replace(html, string.Empty);
#else
    var result = NoScriptRegex.Replace(html, string.Empty);
#endif

    return result;
  }

  /// <summary>
  /// Shortcut method to get next/adjacent element to the one provided.
  /// </summary>
  /// <param name="node">The element/node to operate on.</param>
  /// <returns>
  /// An adjacent element or <c>null</c> of there are no siblings.
  /// </returns>
  public static HtmlNode? NextElement(this HtmlNode? node)
  {
    if (node is not null)
    {
      var sibling = node.NextSibling;
      while (sibling is not null)
      {
        if (sibling.NodeType == HtmlNodeType.Element)
        {
          return sibling;
        }

        sibling = sibling.NextSibling;
      }
    }

    return null;
  }

  /// <summary>
  /// Check if an element is visible.
  /// </summary>
  /// <remarks>
  /// To avoid honey pots and detection, always use this method to check any links for
  /// actual visibility before following the link or simulating a click on it.
  /// </remarks>
  /// <param name="node">The element/node to operate on.</param>
  /// <param name="checkParentNodes">(Optional) <c>true</c> to check parent nodes, <c>false</c> to only check the element/node directly.</param>
  /// <returns>
  /// <c>true</c> if the element/node is visible, <c>false</c> if not.
  /// </returns>
  public static bool IsVisible(this HtmlNode? node, bool checkParentNodes = true)
  {
    if (node is null)
    {
      return false;
    }

    var attribute = node.Attributes["style"];

    bool thisVisible = false;

    if (attribute is null || CheckStyleVisibility(attribute.Value))
    {
      thisVisible = true;
    }

    if (thisVisible && node.ParentNode is not null && checkParentNodes)
    {
      return IsVisible(node.ParentNode, checkParentNodes);
    }

    return thisVisible;
  }

  private static bool CheckStyleVisibility(string style)
  {
    if (string.IsNullOrWhiteSpace(style))
    {
      return true;
    }

    var keys = ParseHtmlStyleString(style);

    if (keys.Keys.Contains("display"))
    {
      var display = keys["display"];
      if (display?.Equals("none", StringComparison.OrdinalIgnoreCase) == true)
      {
        return false;
      }
    }

    if (keys.Keys.Contains("visibility"))
    {
      var visibility = keys["visibility"];
      if (visibility?.Equals("hidden", StringComparison.OrdinalIgnoreCase) == true)
      {
        return false;
      }
    }

    return true;
  }

  private static IDictionary<string, string> ParseHtmlStyleString(string style)
  {
    var result = new Dictionary<string, string>();

    style = style.Replace(" ", string.Empty).ToLowerInvariant();

    foreach (string s in style.Split([';'], StringSplitOptions.RemoveEmptyEntries))
    {
      if (!s.Contains(":"))
      {
        continue;
      }

      var data = s.Split(':');
      result.Add(data[0], data[1]);
    }

    return result;
  }

#if NET7_0_OR_GREATER
  [GeneratedRegex(@"([^\d\.])*", RegexOptions.None, 1000)]
  private static partial Regex NumbersOnlyRegex();

  [GeneratedRegex(@"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", RegexOptions.IgnoreCase, 5000)]
  private static partial Regex NoScriptRegex();
#endif
}
