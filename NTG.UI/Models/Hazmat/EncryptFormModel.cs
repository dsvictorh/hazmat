using NTG.Logic.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace NTG.UI.Models.Hazmat
{
    public class EncryptFormModel : BaseFormModel<EncryptFormModel>
    {
        [Required]
        public string Text { get; set; }
        public string EncryptedText { get; private set; }

        protected override void OnSubmit()
        {
            EncryptedText = EncryptionService.Encrypt(Text);
        }
    }
}