using System;
using System.Windows.Input;
using Caliburn.Micro;
using GUI.Command;
using GUI.Models;
using GUI.Models.Schema;

namespace GUI.ViewModels
{
    public class TransformerFormViewModel : Screen
    {
        private readonly Action<TransformerFormData> onSubmit;
        private bool canSubmit = true;
        private TransformerFormData formData;

        public TransformerFormViewModel(TransformerModel transformer, Action<TransformerFormData> onSubmit)
        {
            this.onSubmit = onSubmit;
            formData = new TransformerFormData(transformer);

            HandleSubmitCommand = new MyICommand(HandleSubmit);
        }

        public bool CanSubmit
        {
            get => canSubmit;
            set
            {
                canSubmit = value;
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        public TransformerFormData FormData
        {
            get => formData;
            set
            {
                formData = value;
                NotifyOfPropertyChange(() => FormData);
            }
        }

        public ICommand HandleSubmitCommand { get; set; }

        public void HandleSubmit(object parameter)
        {
            CanSubmit = false;

            onSubmit?.Invoke(FormData);

            TryClose(true);
        }
    }
}
