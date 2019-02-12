using Syncfusion.XForms.DataForm;
using Syncfusion.XForms.DataForm.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace HandlingCustomEditorValue
{
    public class DataFormBehavior : Behavior<ContentPage>
    {
        SfDataForm dataForm = null;
        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);
            dataForm = bindable.FindByName<SfDataForm>("dataForm");
            dataForm.RegisterEditor("numeric", new CustomTextEditor(dataForm));
            dataForm.RegisterEditor("Age", "numeric");
            dataForm.ValidationMode = ValidationMode.LostFocus;
            dataForm.DataObject = new ContactForm();

        }
    }

    public class CustomTextEditor : DataFormEditor<Entry>
    {
        public CustomTextEditor(SfDataForm dataForm) : base(dataForm)
        {
        }

        protected override Entry OnCreateEditorView(DataFormItem dataFormItem)
        {
            return new Entry();
        }
        protected override void OnInitializeView(DataFormItem dataFormItem, Entry view)
        {
            base.OnInitializeView(dataFormItem, view);
            view.Keyboard = Keyboard.Numeric;
        }

        protected override void OnWireEvents(Entry view)
        {
            view.TextChanged += View_TextChanged;
            view.Focused += View_Focused;
            view.Unfocused += View_Unfocused;
        }

        private void View_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnValidateValue(sender as Entry);
        }

        private void View_Focused(object sender, FocusEventArgs e)
        {
            var view = (sender as Entry);
            view.TextColor = Color.Green;
        }

        protected override bool OnValidateValue(Entry view)
        {
            return this.DataForm.Validate("Age");
        }
        private void View_Unfocused(object sender, FocusEventArgs e)
        {
            var view = sender as Entry;
            view.TextColor = Color.Red;

            if (this.DataForm.CommitMode == Syncfusion.XForms.DataForm.CommitMode.LostFocus || this.DataForm.ValidationMode == ValidationMode.LostFocus)
                this.OnValidateValue(view);
            if (this.DataForm.CommitMode != Syncfusion.XForms.DataForm.CommitMode.LostFocus) return;
            this.OnCommitValue(view);
            OnValidateValue(sender as Entry);
        }
        private void View_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = sender as Entry;
            if (DataForm.CommitMode == Syncfusion.XForms.DataForm.CommitMode.PropertyChanged || DataForm.ValidationMode == ValidationMode.PropertyChanged)
                this.OnValidateValue(view);
            if (this.DataForm.CommitMode != Syncfusion.XForms.DataForm.CommitMode.PropertyChanged) return;
            this.OnCommitValue(view);
        }

        protected override void OnCommitValue(Entry view)
        {
            var dataFormItemView = view.Parent as DataFormItemView;
            this.DataForm.ItemManager.SetValue(dataFormItemView.DataFormItem, view.Text);
        }

        protected override void OnUnWireEvents(Entry view)
        {
            view.TextChanged -= View_TextChanged;
            view.Focused -= View_Focused;
            view.Unfocused -= View_Unfocused;
        }
    }
    public class ContactForm : INotifyPropertyChanged
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        private int age;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Age should not be empty")]
        public int Age
        {
            get
            {
                return age;
            }
            set
            {
                age = value;
                RaisePropertyChanged("Age");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(String Name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

    }
}
