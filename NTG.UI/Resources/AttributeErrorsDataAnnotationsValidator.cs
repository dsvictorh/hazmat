using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NTG.UI.Resources
{
    public class AttributeErrorsDataAnnotationsValidator : DataAnnotationsModelValidator<ValidationAttribute>
    {
        /// <summary>
        /// The type of the resource which holds the error messqages
        /// </summary>
        public static Type ResourceType { get; set; }

        /// <summary>
        /// Function to get the ErrorMessageResourceName from the Attribute
        /// </summary>
        public static Func<ValidationAttribute, string> ResourceNameFunc
        {
            get { return _resourceNameFunc; }
            set { _resourceNameFunc = value; }
        }
        private static Func<ValidationAttribute, string> _resourceNameFunc = attr => attr.GetType().Name;

        public AttributeErrorsDataAnnotationsValidator(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute)
            : base(metadata, context, attribute)
        {
            if (Attribute.ErrorMessageResourceType == null)
            {
                this.Attribute.ErrorMessageResourceType = ResourceType;
            }

            if (Attribute.ErrorMessageResourceName == null)
            {
                this.Attribute.ErrorMessageResourceName = ResourceNameFunc(this.Attribute);
            }
        }
    }
}