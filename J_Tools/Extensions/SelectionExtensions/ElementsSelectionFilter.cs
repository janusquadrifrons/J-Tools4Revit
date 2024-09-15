using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

// Element Filtering Delegation to Command Functions

namespace J_Tools.Extensions.SelectionExtensions
{
    public class ElementsSelectionFilter : ISelectionFilter
    {
        private readonly Func<Element, bool> _validateElement;

        public ElementsSelectionFilter(Func<Element, bool> validateElement)
        {
            _validateElement = validateElement; // --- _ kullanımı yerine this.validateElement yerine yapıldı
        }


        public bool AllowElement(Element elem)
        {
            return _validateElement(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
            // return reference.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_SURFACE; // to PointOnELement selection filterind
        }
    }
}
