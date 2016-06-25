using BD2.Model;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BD2.ViewModels
{
    public enum Gender
    {
        Kobieta,
        Mezczyzna
    }
    [ImplementPropertyChanged]
    public class PersonViewModel
    {
        public string Name { get; set; }
        public DateTime DayOfBirth { get; set; }
        public DateTime DayOfDeath { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public Gender Gender { get; set; }
        public IList<Gender> Genders
        {
            get
            {
                return Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList<Gender>();
            }

        }
        public string Father { get; set; }
        public string Mother { get; set; }
        public bool IsConfirm { get; set; }
        public RelayCommand SaveCommand { get; set; }

        public PersonViewModel()
        {
            SaveCommand = new RelayCommand(Save);
        }
        private void Save(object obj)
        {
            IsConfirm = true;
        }

    }
}
