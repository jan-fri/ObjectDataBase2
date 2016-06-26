using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BD2.Model;
using BD2.Repository;
using BD2.Views;
using PropertyChanged;

namespace BD2.ViewModels
{
    [ImplementPropertyChanged]
    public class MainWindowViewModel
    {
        PersonRepository db = new PersonRepository();
        public int CountPerson { get; set; }
        public int CountChild { get; set; }
        public ICommand AddPersonCommand { get; set; }
        public ICommand DeletePersonCommand { get; set; }
        public ICommand EditPersonCommand { get; set; }
        public ICommand AddChildCommand { get; set; }
        public ICommand DeleteChildCommand { get; set; }
        public ObservableRangeCollection<Person> ListsOfPersons { get; set; }
        public Person SelectedPerson { get; set; }
        public Person SelectedPersonCompare1 { get; set; }
        public Person SelectedPersonCompare2 { get; set; }
        public Child SelectedChild { get; set; }
        public List<Child> ListOfChilds
        {
            get
            {
                return SelectedPerson?.Children;
            }
        }

        public List<Child> ListOfSuccessors
        {
            get
            {
                if (SelectedPerson != null)
                {
                    List<Child> childs = new List<Child>();
                    foreach (var child in SelectedPerson?.Children)
                    {
                        if (child.child.DeathDate != "01/01/0001")
                        {
                            foreach (var item in child.child.Children)
                            {
                                childs.Add(item);
                            }
                        }
                        else
                        {
                            childs.Add(child);
                        }
                    }
                    return childs;
                }
                return null;
            }
        }

        public List<string> ListOfAncestors
        {
            get
            {
                if (SelectedPerson != null && SelectedPersonCompare1 != null && SelectedPersonCompare2 != null)
                {
                    if (SelectedPersonCompare1.Name == SelectedPersonCompare2.Name)
                    {
                        MessageBox.Show("Zostały wybrane te same osoby");
                        SelectedPersonCompare1 = null;
                        SelectedPersonCompare2 = null;
                        return null;
                    }
                    List<string> listOfAncestors1 = new List<string>();
                    List<string> listOfAncestors2 = new List<string>();
                    GetAncesotor(listOfAncestors1, SelectedPersonCompare1);
                    GetAncesotor(listOfAncestors2, SelectedPersonCompare2);


                    List<string> sharedAncestors = new List<string>();
                    foreach (var person1 in listOfAncestors1)
                    {
                        foreach (var person2 in listOfAncestors2)
                        {
                            if (person1 == person2 && !sharedAncestors.Contains(person1) && !sharedAncestors.Contains(person2))
                            {
                                sharedAncestors.Add(person1);
                            }
                        }
                    }
                    return sharedAncestors;               
                }
                return null;
            }
        }
        public void GetAncesotor(List<string> listofAncestors, Person person)
        {
            if (person.Father != null)
            {
                listofAncestors.Add(person.Father);
                person = ListsOfPersons.Where(x => x.Name == person.Father).FirstOrDefault();
                GetAncesotor(listofAncestors, person);
            }
            if (person.Mother != null)
            {
                listofAncestors.Add(person.Mother);
                person = ListsOfPersons.Where(x => x.Name == person.Mother).FirstOrDefault();
                GetAncesotor(listofAncestors, person);
            }
        }

        public List<TreeViewItem> TreeView
        {
            get
            {
                return new List<TreeViewItem> { CreateTree(new Child { child = SelectedPerson, Count = 1 }, 1, true) };
            }
        }

        public string SelectedBirthDate
        {
            get
            {
                if (SelectedPerson != null)
                    return SelectedPerson.BirthDate;
                else
                    return null;


            }
        }

        public string SelectedDeathDate
        {
            get
            {
                if (SelectedPerson != null)
                {
                    if (SelectedPerson.DeathDate == "01/01/0001")
                    {
                        return "brak";
                    }
                    return SelectedPerson.DeathDate;
                }

                else
                    return null;
            }
        }

        public string SelectedGender
        {
            get
            {
                if (SelectedPerson != null)
                    return SelectedPerson.Gender;
                else
                    return null;
            }
        }
        public string SelectedAge
        {
            get
            {
                if (SelectedPerson != null)
                {
                    var birthDate = DateTime.Parse(SelectedPerson.BirthDate);

                    if (SelectedPerson.DeathDate == "01/01/0001")
                    {
                        return (DateTime.Now.Year - birthDate.Year).ToString("d");
                    }
                    else
                    {
                        var deathDate = DateTime.Parse(SelectedPerson.DeathDate);
                        var age = (deathDate.Year - birthDate.Year);
                        return age.ToString("d");
                    }
                }
                else
                    return null;

            }
        }

        public string SelectedFather
        {
            get
            {
                if (SelectedPerson != null)
                {
                    return SelectedPerson.Father;
                }
                else
                    return null;
            }
        }

        public string SelectedMother
        {
            get
            {
                if (SelectedPerson != null)
                    return SelectedPerson.Mother;
                else
                    return null;
            }
        }


        public MainWindowViewModel()
        {
            ListsOfPersons = new ObservableRangeCollection<Person>();
            AddPersonCommand = new RelayCommand(AddPerson);
            DeletePersonCommand = new RelayCommand(DeletePerson, CanExecutePerson);
            EditPersonCommand = new RelayCommand(EditPerson, CanExecutePerson);
            AddChildCommand = new RelayCommand(AddChild, CanExecutePerson);
            DeleteChildCommand = new RelayCommand(DeleteChild, CanExecuteChild);
            Refresh();
        }

        private bool CanExecuteChild(object obj)
        {
            if (SelectedChild == null)
                return false;
            return true;
        }

        private bool CanExecutePerson(object obj)
        {
            if (SelectedPerson == null)
                return false;
            return true;
        }

        private void Clear()
        {
            ListsOfPersons.Clear();
            SelectedChild = null;
            SelectedPerson = null;
        }

        private void Refresh()
        {
            CountPerson = db.GetPerson<Person>().Count;
            CountChild = db.GetPerson<Child>().Count;
            string name = SelectedChild?.child.Name;
            string listName = SelectedPerson?.Name;
            Clear();
            ListsOfPersons.AddRange(db.GetPerson<Person>());
            SelectedPerson = ListsOfPersons.FirstOrDefault(x => x.Name == listName) ?? ListsOfPersons.FirstOrDefault();
            SelectedChild = SelectedPerson?.Children.FirstOrDefault(x => x.child.Name == name);
            if (SelectedChild != null)
                return;
            SelectedChild = SelectedPerson?.Children.FirstOrDefault();
        }
        private void AddPerson(object obj)
        {
            Person newPerson = new Person();
            var viewmodel = new PersonViewModel
            {
                Name = string.Empty,

            };
            AddPersonView addListView = new AddPersonView { DataContext = viewmodel };
            addListView.ShowDialog();
            if (viewmodel.IsConfirm)
            {
                var exist = db.DownloadList(viewmodel.Name);
                if (exist != null)
                {

                    MessageBox.Show("Istnieje już taka osoba!");
                    return;
                }
                newPerson.Name = viewmodel.Name;

                var date = viewmodel.DayOfBirth.Date;
                newPerson.BirthDate = date.ToString("d");
                date = viewmodel.DayOfDeath.Date;
                newPerson.DeathDate = date.ToString("d");
                newPerson.Gender = viewmodel.Gender.ToString();
                var res = db.AddPerson(newPerson);
                if (res == false)
                {
                    MessageBox.Show("Wystąpił błąd przy dodawaniu!");
                }
                Refresh();
            }
        }

        private void EditPerson(object obj)
        {
            var viewmodel = new PersonViewModel
            {
                Name = SelectedPerson.Name,
                DayOfBirth = DateTime.Parse(SelectedPerson.BirthDate),
                DayOfDeath = DateTime.Parse(SelectedPerson.DeathDate),
                Gender = (Gender)Enum.Parse(typeof(Gender), SelectedPerson.Gender)
            };
            EditList view = new EditList { DataContext = viewmodel };
            view.ShowDialog();
            if (viewmodel.IsConfirm)
            {
                var birthDate = viewmodel.DayOfBirth.Date;
                var deathDate = viewmodel.DayOfDeath.Date;
                viewmodel.BirthDate = birthDate.ToString("d");
                viewmodel.DeathDate = deathDate.ToString("d");
                var gender = viewmodel.Gender.ToString();

                if (SelectedPerson.Children.Any())
                {
                    Person newPerson = new Person
                    {
                        BirthDate = birthDate.ToString("d"),
                        DeathDate = deathDate.ToString("d"),
                        Gender = gender
                    };

                    foreach (var child in SelectedPerson.Children)
                    {
                        if (!CheckIfValid(newPerson, child, false))
                        {
                            return;
                        } 
                    }
                }
                var result = db.EditPerson(SelectedPerson, viewmodel.Name, viewmodel.BirthDate, viewmodel.DeathDate, gender);
                if (result == false)
                {
                    MessageBox.Show("Wystąpił błąd przy edycji jednostki");
                }
                Refresh();
            }
        }

        private void DeletePerson(object obj)
        {
            var result = db.DeletePerson(SelectedPerson);
            if (result == false)
            {
                MessageBox.Show("Wystąpił błąd przy usuwaniu!");
            }
            Refresh();
        }


        private void DeleteChild(object obj)
        {
            var result = db.DeleteChildFromList(SelectedPerson, SelectedChild);
            if (result == false)
            {
                MessageBox.Show("Wystąpił błąd przy usuwaniu");
            }
            Refresh();
        }


        private void AddChild(object obj)
        {
            ChildViewModel viewModel = new ChildViewModel
            {
                Count = "1",
                IsAdd = true,
                Persons = ListsOfPersons,
                SelectedPerson = ListsOfPersons.FirstOrDefault()
            };
            AddChildView view = new AddChildView { DataContext = viewModel };
            Child newChild = new Child();
            view.ShowDialog();
            if (viewModel.IsConfirm)
            {
                newChild.child = viewModel.SelectedPerson;
                newChild.Count = Convert.ToDecimal(viewModel.Count);

                if (CheckIfValid(SelectedPerson, newChild))
                {
                    var res = db.AddChild(SelectedPerson, newChild);
                    if (res == false)
                    {
                        MessageBox.Show("Wystąpił błąd przy dodawaniu!");
                    }
                    Refresh();
                }
            }
        }
        
        private bool CheckIfValid(Person SelectedPerson, Child newChild, bool addNewChild=true)
        {
            if (addNewChild)
            {
                if (!db.CheckIfChildExists(SelectedPerson, newChild))
                {
                    MessageBox.Show("Takie dziecko juz istnieje");
                    return false;
                }

                if (db.CheckCycles(SelectedPerson, newChild))
                {
                    MessageBox.Show("Nie jest możliwe dodanie dziecka. (dziecko jest rodzicem?)");
                    return false;
                }

                if (!db.CheckChildParents(SelectedPerson, newChild))
                {
                    MessageBox.Show("Dziecko ma juz rodzica o danej plci");
                    return false;
                } 
            }

            if (SelectedPerson.Gender == Gender.Kobieta.ToString())
            {
                if (!db.CheckParentAge(SelectedPerson, newChild, Gender.Kobieta))
                {
                    MessageBox.Show("Wiek matki powinien być między 10 a 60 lat w momencie urodzenia dziecka");
                    return false;
                }

                if (!db.CheckIfPossibleToBeBorn(SelectedPerson, newChild, Gender.Kobieta))
                {
                    MessageBox.Show("Dziecko nie urodziło się za życia matki");
                    return false;
                }

            }

            if (SelectedPerson.Gender == Gender.Mezczyzna.ToString())
            {
                if (!db.CheckParentAge(SelectedPerson, newChild, Gender.Mezczyzna))
                {
                    MessageBox.Show("Wiek ojca powinien być między 12 a 70 lat w momencie urodzenia dziecka");
                    return false;
                }

                if (!db.CheckIfPossibleToBeBorn(SelectedPerson, newChild, Gender.Mezczyzna))
                {
                    MessageBox.Show("Dziecko urodziło się więcej niż 270 dni przed śmiercią ojca");
                    return false;
                }

            }
            return true;
        }

        private TreeViewItem CreateTree(Child element, decimal count, bool root = false)
        {
            if (element?.child == null)
            {
                return new TreeViewItem();
            }
            var newitem = new TreeViewItem { Header = element.child?.Name };
            if (root == false)
            {
                newitem.Header = element.child?.Name;
            }

            foreach (var child in element.child.Children)
            {
                newitem.Items.Add(CreateTree(child, element.Count * count));

            }

            return newitem;
        }


    }
}
