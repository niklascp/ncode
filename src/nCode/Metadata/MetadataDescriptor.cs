using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nCode.UI;
using System.Web.Compilation;
using System.Web.UI;

namespace nCode.Metadata
{
    public static class EditControlBindings
    {
        static EditControlBindings()
        {
            Bindings = new Dictionary<Type, string>();
        }

        public static IDictionary<Type, string> Bindings { get ; private set; } 
    }

    public class MetadataDescriptor
    {
        public MetadataDescriptor(string name, string editControlPath)
        {
            Name = name;
            EditControlPath = editControlPath;
        }

        public string Name { get; private set; }        

        public string EditControlPath { get; private set; }

        public string EditControlArguments { get; set; }

        public string DisplayText { get; set; }

        public MetadataEditControlViewMode ViewMode { get; set; }

        /// <summary>
        /// Gets an user control that can be used to edit this metadata.
        /// </summary>
        public virtual UserControl GetEditControl()
        {
            var control = (UserControl)BuildManager.CreateInstanceFromVirtualPath(EditControlPath, typeof(UserControl));

            if (control is MetadataEditControl)
                ((MetadataEditControl)control).MetadataDescriptor = this;

            return control;
        }
    }

    public class MetadataDescriptor<T> : MetadataDescriptor
    {
        public MetadataDescriptor(string name)
            : base(name, null)
        {
            
        }

        /// <summary>
        /// Gets an user control that can be used to edit this metadata.
        /// </summary>
        public override UserControl GetEditControl()
        {
            var ps = typeof(T).GetCustomAttributes(typeof(EditControlAttribute), false);
            var editControlPath = (string)null;

            if (ps.Any())
            {
                editControlPath = ((EditControlAttribute)ps.First()).EditControlPath;
            }
            else if (EditControlBindings.Bindings.ContainsKey(typeof(T)))
            {
                editControlPath = EditControlBindings.Bindings[typeof(T)];
            }
            else
            {
                throw new ApplicationException(string.Format("Could not find a Edit Control for the type '{0}'. Add an EditControlAttribute to the type or a default binding for the type to EditControlBindings.", typeof(T).ToString()));
            }

            var control = (UserControl)BuildManager.CreateInstanceFromVirtualPath(editControlPath, typeof(UserControl));

            if (control is MetadataEditControl)
                ((MetadataEditControl)control).MetadataDescriptor = this;

            return control;
        }
    }

    public enum MetadataEditControlViewMode
    {
        Inline,
        Block
    }
}


