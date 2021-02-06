using System;
using System.Windows.Input;
using Caliburn.Micro;
using GUI.Core.Tree;
using GUI.Core.Tree.Helpers;
using GUI.Models;
using GUI.ViewModels;

namespace GUI.Command
{
    public class OpenTransformerFormCommand : ICommand
    {
        private readonly EquipmentTreeNode node;

        public event EventHandler CanExecuteChanged;

        public OpenTransformerFormCommand(EquipmentTreeNode node)
        {
            this.node = node;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var windowManager = IoC.Get<IWindowManager>();

            var transformerModel = node.Item as TransformerModel;
            var formViewModel = new TransformerFormViewModel(transformerModel, onSubmit: UpdateNode);

            windowManager.ShowDialog(formViewModel);
        }

        private void UpdateNode(TransformerFormData formData)
        {
            var transformerModel = node.Item as TransformerModel;
            var tapChanger = transformerModel.RatioTapChanger;

            tapChanger.NormalStep = formData.NormalStep;
        }
    }
}
