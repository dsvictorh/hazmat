using NTG.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTG.UI.Models
{
    public class BaseFormModel<T> : BaseAjaxModel where T : class
    {
        public bool Success { get; private set; }
        

        protected virtual void OnSubmit() {}

        public void Submit()
        {
            if (Validate())
            {
                OnSubmit();
                Success = !Messages.Any(m => m.Value.Count(x => x.MessageType != MessageTypes.Success) > 0);
            }
        }

        private bool Validate()
        {
            var context = new ValidationContext(this as T, serviceProvider: null, items: null);
            var validations = new List<ValidationResult>();
            var result = Validator.TryValidateObject(this as T, context, validations, true);
            var property = "";

            validations.ForEach(validation => {
                property = validation.MemberNames.FirstOrDefault();
                AddMessage(property + GetErrorMessageSufix(), new Message(validation.ErrorMessage, MessageTypes.Warning));
            });

            return result && OnValidate();
        }

        protected virtual bool OnValidate()
        {
            return true;
        }

        protected virtual string GetErrorMessageSufix()
        {
            return string.Empty;
        }
    }
}