using System;
using System.Collections.ObjectModel;
using System.Globalization;
using BD2.Model;
using PropertyChanged;

namespace BD2.ViewModels
{
    [ImplementPropertyChanged]
    public class ChildViewModel
    {
        public ObservableCollection<Person> Persons { get; set; }
        public Person SelectedPerson { get; set; }
        public string Count { get; set; }      
        public bool IsAdd { get; set; }
        public bool IsConfirm { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public ChildViewModel()
        {
            SaveCommand = new RelayCommand(Save);
        }
        private void Save(object obj)
        {
            IsConfirm = true;
        }
    }
}
