using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Collections.Specialized;
using System.Xml;

namespace XmlProc
{
    public class ElementInfo
    {
        public ElementInfo() { }
        public ElementInfo(string prefix, string name, string nsUri, bool empty, int depth) { Name = name; Prefix = prefix; NamespaceUri = nsUri; IsEmpty = empty; Depth = depth; }
        public string Name;
        public string Prefix;
        public string NamespaceUri;
        public bool IsEmpty;
        public int Depth;
        public Dictionary<string, AttributeInfo> Attributes = new Dictionary<string, AttributeInfo>();
        /// <summary>
        /// Tu mo�emy w�o�y� dowolny obiekt - pole nieu�ywane przez engine
        /// </summary>
        public object ContextValue;
        public AttributeInfo GetAttribute(string name)
        {
            if (!Attributes.ContainsKey(name)) return null;
            return Attributes[name];
        }
        public string GetAttributeValue(string name)
        {
            AttributeInfo ai = GetAttribute(name);
            return ai == null ? null : ai.Value;
        }

        /// <summary>
        /// handler kt�ry obs�uguje ten element. Uwaga: poprawna warto�� w tym polu jest wy��cznie
        /// podczas przetwarzania elementu oraz jego element�w zagnie�dzonych. 
        /// </summary>
        public IElementHandler Handler;
    }

    public class AttributeInfo
    {
        public string Name;
        public string Value;
        public string Prefix;
        public string NamespaceUri;
    }

    /// <summary>
    /// Kontekst aktualnie przetwarzanego elementu
    /// </summary>
    public interface IFormHandlerContext
    {
        ///<summary>Pobierz aktualnie przetwarzany element XML-a</summary>
        XPathNavigator GetCurrentNodeNavigator();
        /// <summary>Ewaluator wyra�e�</summary>
        object Eval(string expr);
        /// <summary>Aktualnie przetwarzany element</summary>
        ElementInfo CurrentElement { get; }
        /// <summary>
        /// Czy pomin�� zawarto�� bie��cego elementu (ustaw na true je�li handler ju� obs�u�y� zawarto�� aktualnego elementu i nie powinna ona by�
        /// dalej przetwarzana). 
        /// </summary>
        bool SkipElementContent { get; set; }
        /// <summary>Output</summary>
        XmlWriter Output { get; }
        /// <summary>Wyci�gnij element ze stosu</summary>
        ElementInfo PeekElementStack(int pos);
        /// <summary>Rozmiar stosu element�w</summary>
        int ElementStackSize { get; }
        /// <summary>
        /// Aktualnie 'obslugiwany' obiekt.
        /// </summary>
        object Root { get; set; }
        /// <summary>Pobranie kolejnego identyfikatora dla elementu GUI</summary>
        string GetNextId();
    }

    public interface IElementHandler
    {
        /// <summary>
        /// Warunki wej�ciowe: reader (Input) stoi tu� za deklaracj� elementu przetwarzanego
        /// Je�li nie chcemy przetwarza� wn�trza elementu, nie czytamy NIC z Inputu (wtedy wn�trze elementu b�dzie przetworzone automatycznie)
        /// Je�li przetwarzamy wn�trze elementu powinni�my przeczyta� tyle, aby 
        /// stan�� NA tagu zamykaj�cym element przetwarzany (NodeType w inpucie powinno by� 'EndElement')
        /// </summary>
        void ElementStart(IFormHandlerContext context);
        /// <summary>
        /// Wywo�ywane po napotkaniu tagu zamykaj�cego element przetwarzany
        /// Uwaga: je�li sami przetworzyli�my jego zawarto�� to ta funkcja nie b�dzie wywo�ana
        /// W przypadku pustych element�w funkcja b�dzie i tak wywo�ana.
        /// </summary>
        void ElementEnd(IFormHandlerContext context);

    }

    /// <summary>
    /// Dane wej�ciowe do generowanego formularza.
    /// </summary>
    public class FormHandlerInput
    {
        private NameValueCollection _params = new NameValueCollection();

        public FormHandlerInput()
        {
        }
        
        public NameValueCollection Params
        {
            get { return _params; }
            set { _params = value; }
        }
    }


    /// <summary>
    /// Generator forms�w
    /// </summary>
    public interface IFormHandler
    {
        void Process(FormHandlerInput input, XmlWriter output);
        void Process(NameValueCollection parameters, XmlWriter output);
        string Process(FormHandlerInput input);
        string FormName { get; }
    }

    public class Helper
    {
        public const string PreprocessNamespace = "http://www.rg.com/preprocess";
        public const string TargetNamespace = "http://www.rg.com";
    }
}
