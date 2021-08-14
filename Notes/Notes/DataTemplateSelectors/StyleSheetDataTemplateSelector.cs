using Notes.Models;
using Xamarin.Forms;

namespace Notes.DataTemplateSelectors
{
    public class StyleSheetDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CSSTemplate_NameOnly { get; set; }
        public DataTemplate CSSTemplate_ReadOnly { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var sheet = (CSS)item;

            if (sheet.IsReadOnly)
            {
                return CSSTemplate_ReadOnly;
            }
            return CSSTemplate_NameOnly;
        }
    }
}
