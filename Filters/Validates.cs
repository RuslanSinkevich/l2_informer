using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Informer.Filters
{
    /// <summary>
    /// Проверяет, что HTML не содержит несанкционированные элементы, атрибуты и JavaScript.
    /// </summary>
    public class HtmlScrubber
    {
        #region Static Default

        /// <summary>
        /// Gets the allowed attributes that apply to all HTML tags.
        /// </summary>
        /// <returns>Gets the allowed attributes that apply to all HTML tags.</returns>
        static string DefaultAttributes()
        {
            return "class,style,id";
        }

        /// <summary>
        /// Получает Разрешенные HTML теги и их разрешенных атрибутов.
        /// </summary>
        /// <returns>Gets the allowed HTML tags and their respective allowed attributes.</returns>
        static NameValueCollection DefaultTags()
        {
            var defaultTags = new NameValueCollection
                {
                    {"h1", "align"},
                    {"h2", "align"},
                    {"h3", "align"},
                    {"h4", "align"},
                    {"h5", "align"},
                    {"h6", "align"},
                    {"strong", ""},
                    {"em", ""},
                    {"u", ""},
                    {"loc", ""},
                    {"s", ""},
                    {"b", ""},
                    {"i", ""},
                    {"strike", ""},
                    {"sup", ""},
                    {"sub", ""},
                    {"font", "color,size,face"},
                    {"blockquote", "dir"},
                    {"ul", ""},
                    {"ol", ""},
                    {"li", ""},
                    {"p", "align,dir"},
                    {"address", ""},
                    {"pre", ""},
                    {"div", "align"},
                    {"hr", ""},
                    {"br", ""},
                    {"a", "href,target,name,title"},
                    {"span", "align"},
                    {"table", "border,cellpadding,cellspacing,class,style,id"},
                    {"tbody", ""},
                    {"tr", ""},
                    {"th", "scope"},
                    {"td", ""},
                    {"img", "src,alt,title,npcId,npcId,loc"}
                };

            // Add the default attributes that apply to all HTML tags.
            var defaultAttributes = DefaultAttributes();
            foreach (var key in defaultTags.AllKeys)
            {
                defaultTags.Add(key, defaultAttributes);
            }

            return defaultTags;
        }
        private readonly NameValueCollection _allowedTags;
        static readonly Regex Regex = new Regex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        static readonly Regex JsAttributeRegex = new Regex("javascript:", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static readonly Regex XmlLineBreak = new Regex("&#x([DA9]|20|85|2028|0A|0D)(;)?", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static readonly Regex FilterdCharacters = new Regex("\\=|\\\"|\\'|\\s|\"'", RegexOptions.Compiled);
        static readonly Regex ValidProtocols = new Regex("^((http(s)?|mailto|mms):|/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static readonly Regex BannedChars = new Regex("\\s", RegexOptions.Compiled);
        #endregion

        #region Public Statics

        /// <summary>
        /// Очистите потенциально опасных HTML и JavaScript по желанию удалить из указанной строки. все
        /// Теги и атрибуты сравнивается со списком приемлемых теги и атрибуты. Атрибуты не
        /// В белом списке удаляются. Если encodeExceptions = TRUE, непризнанные теги HTML закодированы. Если ложно,
        /// Теги удаляются (но не внутренний текст тега).
        /// </summary>
        /// <param name="stringToClean">Строка чистить.</param>
        /// <param name="encodeExceptions">Значение, указывающее, HTML-кодировать любой HTML-теги нет
        /// В список допустимых тегов HTML. Если ложно, непризнанной теги будут удалены, но содержание
        /// Теги сохраняются.</param>
        /// <param name="filterScripts">Значение, указывающее, следует ли удалить встроенные JavaScript. истинный
        /// Величина указывает Javacript удаляется.</param>
        /// <returns>
        /// Возвращает строку с потенциально опасными HTML безопасным.
        /// </returns>
        public static string Clean(string stringToClean, bool encodeExceptions, bool filterScripts)
        {
            if (string.IsNullOrEmpty(stringToClean))
                return stringToClean;

            var f = new HtmlScrubber(stringToClean, encodeExceptions, filterScripts);
            return f.Clean();
        }

        /// <summary>
        /// Очистите потенциально опасных HTML, так что несанкционированные HTML и Javascript удаляется. Если конфигурация
        /// Установка allowHtmlInTitlesAndCaptions верно, то вход "cleaned", так что все HTML теи, которые не находятся в
        /// Предопределенного списка приемлемых HTML теги HTML кодировке, и все атрибуты не найден в белом списке
        /// Удаляются (например OnClick, OnMouseOver). Если allowHtmlInTitlesAndCaptions = "ложных", то все HTML теги
        /// Удалены. Вне зависимости от параметров конфигурации, все <script/> теги бежал и все экземпляры
        /// "JavaScript:" удаляются.
        ///</summary>
        ///<param name="html">Строка, содержащую потенциально опасных HTML-теги.</param>
        ///<returns>Возвращает строку с потенциально опасными HTML тегами HTML-закодированные или удалены.</returns>
        public static string SmartClean(string html)
        {
            string cleanHtml = GalleryServerPro.Configuration.ConfigManager.GetGalleryServerProConfigSection().Core.LockItem ? Clean(html, true, true) : RemoveAllTags(html);

            return cleanHtml;
        }

        /// <summary>
        /// Удалите все теги HTML из заданной строки. Если перегрузка с параметром escapeQuotes верно, то все
        /// Апострофы и кавычки заменяются на "и", так что строка может быть указан в HTML
        /// Атрибуты, такие как теги заголовков. Если escapeQuotes параметр не указан, никакая замена не производится.
        /// </summary>
        /// <param name="html">Строку, содержащую HTML теги удалить.</param>
        /// <returns>Возвращает строку со всеми HTML теги удалены, включая скобки.</returns>
        public static string RemoveAllTags(string html)
        {
            return RemoveAllTags(html, false);
        }

        #pragma warning disable 1570
#pragma warning restore 1570
#pragma warning disable 1570
#pragma warning restore 1570
        /// <summary>
        /// Удалите все теги HTML из заданной строки. Если перегрузка с параметром escapeQuotes true, то все
#pragma warning disable 1570
        /// Апострофы и кавычки заменяются на  &quot; и &apos, так что строка может быть указан в HTML
#pragma warning restore 1570
        /// Атрибуты, такие как теги заголовков. Если escapeQuotes параметр не указан, никакая замена не производится.
        /// </summary>
        /// <param Name="html"> строка, содержащая HTML теги удалить. </param>
        /// Когда
        /// <param name="html"></param>
#pragma warning disable 1570
        /// <param name="escapeQuotes"> true, все апострофы и кавычки заменяются на &quot; и &apos. </param>
#pragma warning restore 1570
        /// <returns> Возвращает строку со всеми HTML теги удалены, включая скобки. </returns>
        public static string RemoveAllTags(string html, bool escapeQuotes)
        {
            if (html == null || html.Trim() == string.Empty)
                return html;

            var f = new HtmlScrubber(html);
            f._allowedTags.Clear();

            string cleanHtml = f.Clean();

            if (escapeQuotes)
            {
                cleanHtml = cleanHtml.Replace("\"", "&quot;");
                cleanHtml = cleanHtml.Replace("'", "&apos;");
            }

            return cleanHtml;
        }

        #endregion

        #region Private statics

        //private static string StripScriptTags(string text)
        //{
        //  return jsAttributeRegex.Replace(text, string.Empty);
        //}

        #endregion

        #region Private members
        string _input;
        readonly StringBuilder _output = new StringBuilder();
        readonly bool _cleanJs;
        bool _isFormatted;
        readonly bool _encodeExceptions;
        #endregion

        #region cnstr

        /// <summary>
        /// Filters unknown markup. Will not encode exceptions
        /// </summary>
        /// <param name="html">Markup to filter</param>
        public HtmlScrubber(string html)
            : this(html, false, true)
        {
        }

        /// <summary>
        /// Filters unknown markup
        /// </summary>
        /// <param name="html">Markup to filter</param>
        /// <param name="encodeRuleExceptions">Should unknown elements be encoded or removed?</param>
        public HtmlScrubber(string html, bool encodeRuleExceptions)
            : this(html, encodeRuleExceptions, true)
        {

        }

        /// <summary>
        /// Filters unknown markup
        /// </summary>
        /// <param name="html">Markup to filter</param>
        /// <param name="encodeRuleExceptions">Should unknown elements be encoded or removed?</param>
        /// <param name="removeScripts">Check for javascript: attributes</param>
        public HtmlScrubber(string html, bool encodeRuleExceptions, bool removeScripts)
        {
            _input = html;
            _cleanJs = removeScripts;
            _encodeExceptions = encodeRuleExceptions;
            _allowedTags = DefaultTags();
        }

        #endregion

        #region Cleaners
        /// <summary>
        /// Returns the results of a cleaning.
        /// </summary>
        /// <returns></returns>
        public string Clean()
        {
            if (!_isFormatted)
            {
                Format();
                _isFormatted = true;
            }
            return _output.ToString();
        }

        #endregion

        #region Format / Walk

        /// <summary>
        /// Walks one time through the HTML. All elements/tags are validated.
        /// The rest of the text is simply added to the internal queue
        /// </summary>
        protected virtual void Format()
        {
            //Lets look for elements/tags
            Match mx = Regex.Match(_input, 0, _input.Length);

            //Never seems to be null
            while (!String.IsNullOrEmpty(mx.Value))
            {
                //find the first occurence of this elment
                int index = _input.IndexOf(mx.Value, StringComparison.Ordinal);

                //add the begining to this tag
                _output.Append(_input.Substring(0, index));

                //remove this from the supplied text
                _input = _input.Remove(0, index);

                //validate the element
                _output.Append(Validate(mx.Value));

                //remove this element from the supplied text
                _input = _input.Remove(0, mx.Length);

                //Get the next match
                mx = Regex.Match(_input, 0, _input.Length);
            }

            //If not Html is found, we should just place all the input into the output container
            if (_input != null && _input.Trim().Length > 0)
                _output.Append(_input);
        }

        #endregion

        #region Validators

        /// <summary>
        /// Main method for starting element validation
        /// </summary>
        /// <param name="tag">A string representing an HTML element tag. Examples: &lt;b&gt;, &lt;br /&gt;, &lt;p class="header"&lt;</param>
        /// <returns>Returns the validated tag.</returns>
        protected string Validate(string tag)
        {
            if (tag.StartsWith("</", StringComparison.Ordinal))
                return ValidateEndTag(tag);

            if (tag.EndsWith("/>", StringComparison.Ordinal))
                return ValidateSingleTag(tag);


            return ValidateStartTag(tag);

        }

        /// <summary>
        /// Validates single element tags such as <br/> and <hr class="X"/>
        /// </summary>
        /// <param name="tag">A string representing a self-enclosed HTML element tag. Example: &lt;br /&gt;</param>
        /// <returns>Returns the validated tag.</returns>
        private string ValidateSingleTag(string tag)
        {
            string strip = tag.Substring(1, tag.Length - 3).Trim();

            var index = strip.IndexOfAny(new[] { ' ', '\r', '\n' });
            if (index == -1)
                index = strip.Length;

            string tagName = strip.Substring(0, index);

            int colonIndex = tagName.IndexOf(":", StringComparison.Ordinal) + 1;

            string safeTagName = tagName.Substring(colonIndex, tagName.Length - colonIndex);

            var allowedAttributes = _allowedTags[safeTagName];
            if (allowedAttributes == null)
                return _encodeExceptions ? Uri.EscapeUriString(tag) : string.Empty;

            string atts = strip.Remove(0, tagName.Length).Trim();

            return ValidateAttributes(allowedAttributes, atts, tagName, "<{0}{1} />");



        }

        /// <summary>
        /// Validates a start tag
        /// </summary>
        /// <param name="tag">A string representing a starting HTML element tag. Examples: &lt;b&gt;, &lt;p class="header"&lt;</param>
        /// <returns>Returns the tag and any valid attributes.</returns>
        protected virtual string ValidateStartTag(string tag)
        {
            //Check for potential attributes
            var endIndex = tag.IndexOfAny(new[] { ' ', '\r', '\n' });

            //simple tag <tag>
            if (endIndex == -1)
                endIndex = tag.Length - 1;

            //Grab the tag name
            string tagName = tag.Substring(1, endIndex - 1);

            //watch for html pasted from Office and messy namespaces
            int colonIndex = tagName.IndexOf(":", StringComparison.Ordinal);
            string safeTagName = tagName;
            if (colonIndex != -1)
                safeTagName = tagName.Substring(colonIndex + 1);


            //Use safe incase a : is present
            var allowedAttributes = _allowedTags[safeTagName];

            //If we do not find a record in the Hashtable, this tag is not valid
            if (allowedAttributes == null)
                return _encodeExceptions ? Uri.EscapeUriString(tag) : string.Empty; //remove element and all attributes if not valid

            //remove the tag name and find all of the current element's attributes
            int start = (colonIndex == -1) ? tagName.Length : safeTagName.Length + colonIndex + 1;

            string attributes = tag.Substring(start + 1, (tag.Length - (start + 2)));

            //if we have attributes, make sure there is no extra padding in the way
            attributes = attributes.Trim();

            //Validate the attributes
            return ValidateAttributes(allowedAttributes, attributes, tagName, "<{0}{1}>");


        }

        /// <summary>
        /// Validates the element's attribute collection
        /// </summary>
        /// <param name="allowedAttributes">The allowed attributes. Example: "href,target,name,title"</param>
        /// <param name="tagAttributes">The tag attributes. Example: "src='mypic.jpg' alt='My photo'"</param>
        /// <param name="tagName">Name of the tag. Examples: p, br, a</param>
        /// <param name="tagFormat">The tag format. Examples: "&lt;{0}{1}&gt;" for a start tag such as &lt;p class="header"&gt;; "&lt;{0}{1} /&gt;"
        /// for a complete tag such as &lt;br /&gt;</param>
        /// <returns>Returns the tag and any valid attributes.</returns>
        protected virtual string ValidateAttributes(string allowedAttributes, string tagAttributes, string tagName, string tagFormat)
        {
            string atts = "";
            // Are there any attributes to validate?
            if (allowedAttributes.Length > 0)
            {
                tagAttributes = XmlLineBreak.Replace(tagAttributes, string.Empty);

                for (int start = 0, end;
                  start < tagAttributes.Length;
                  start = end)
                {
                    //Put the end index at the end of the attribute name.
                    end = tagAttributes.IndexOf('=', start);
                    if (end < 0)
                        end = tagAttributes.Length;
                    //Get the attribute name and see if it's allowed.
                    string att = tagAttributes.Substring(start, end - start).Trim();

                    bool allowed = Regex.IsMatch(allowedAttributes, string.Format(CultureInfo.CurrentCulture, "({0},|{0}$)", att), RegexOptions.IgnoreCase);
                    //Now advance the end index to include the attribute value.
                    if (end < tagAttributes.Length)
                    {
                        //Skip any blanks after the '='.
                        for (++end;
                          end < tagAttributes.Length && (tagAttributes[end] == ' ' || tagAttributes[end] == '\r' || tagAttributes[end] == '\n');
                          ++end)
                        {
                        }
                        if (end < tagAttributes.Length)
                        {
                            //Find the end of the value.
                            end = tagAttributes[end] == '"' //Quoted with double quotes?
                              ? tagAttributes.IndexOf('"', end + 1)
                              : tagAttributes[end] == '\'' //Quoted with single quotes?
                              ? tagAttributes.IndexOf('\'', end + 1)
                              : tagAttributes.IndexOfAny(new[] { ' ', '\r', '\n' }, end); //Otherwise, assume not quoted.
                            //If we didn't find the terminating character, just go to the end of the string.
                            //Otherwise, advance the end index past the terminating character.
                            end = end < 0 ? tagAttributes.Length : end + 1;
                        }
                    }
                    //If the attribute is allowed, copy it.
                    if (allowed)
                    {
                        //Special actions on these attributes. IE will render just about anything that looks like the word javascript:
                        //this includes line breaks, special characters codes, etc.
                        if (att.ToUpperInvariant() == "SRC" || att.ToUpperInvariant() == "HREF")
                        {
                            //File the value of the attribute
                            //string attValue  = tagAttributes.Substring(start + att.Length, end - (start+att.Length)).Trim();
                            string attValue = tagAttributes.Substring(start, end - start).Trim();

                            attValue = attValue.Substring(att.Length);

                            //temporarily remove some characters - mainly =, ", ', and white spaces
                            attValue = FilterdCharacters.Replace(attValue, string.Empty);

                            //validate only http, https, mailto, and / (relative) requests are made
                            if (ValidProtocols.IsMatch(attValue))
                            {
                                atts += ' ' + BannedChars.Replace(tagAttributes.Substring(start, end - start).Trim(), string.Empty);
                            }

                            //If the "if" above fails, we do not render the attribute!

                        }
                        else
                        {
                            atts += ' ' + tagAttributes.Substring(start, end - start).Trim();
                        }


                    }
                }
                //Are we filtering for Javascript?
                if (_cleanJs)
                    atts = JsAttributeRegex.Replace(atts, string.Empty);
            }
            return string.Format(CultureInfo.InvariantCulture, tagFormat, tagName, atts);
        }


        /// <summary>
        /// Validate end/closing tag
        /// </summary>
        /// <param name="tag">A string representing an HTML element end tag. Example: &lt;/p&gt;</param>
        /// <returns></returns>
        protected virtual string ValidateEndTag(string tag)
        {
            string tagName = tag.Substring(2, tag.Length - 3);

            int index = tag.IndexOf(":", StringComparison.Ordinal) - 1;
            if (index == -2)
            {
                index = 0;
            }
            tagName = tagName.Substring(index);
            var allowed = _allowedTags[tagName];

            if (allowed == null)
                return _encodeExceptions ? Uri.EscapeUriString(tag) : string.Empty;

            return tag;

        }

        #endregion
    }
}