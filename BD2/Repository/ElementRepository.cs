using System;
using System.Collections.Generic;
using System.Linq;
using BD2.Model;
using Db4objects.Db4o;
using BD2.ViewModels;

namespace BD2.Repository
{
    public class ElementRepository
    {
        static string _path = @"C:\Users\jasiek\Desktop\Bazy Danych\Zadanie 2\BD2\data";
        private IObjectContainer db;
        public ElementRepository()
        {
            var config = Db4oEmbedded.NewConfiguration();
            config.Common.ActivationDepth = 3;
            config.Common.UpdateDepth = 3;
            db = Db4oEmbedded.OpenFile(config, _path);
        }

        public bool AddPerson(Person person)
        {
            var dbList = GetPerson<Person>().FirstOrDefault(x => x.Name == person.Name);
            if (person == null || dbList != null || person.Name?.Trim() == "")
            {
                return false;
            }
            db.Store(person);
            return true;
        }

        public bool EditPerson(Person person, string newName, string newDayBirth, string newDayDeath, string newGender)
        {
            var dbPerson = GetPerson<Person>().FirstOrDefault(x => x.Name == person.Name);
            if (person == null || dbPerson == null)
            {
                return false;
            }

            dbPerson.Name = newName;
            dbPerson.BirthDate = newDayBirth;
            dbPerson.DeathDate = newDayDeath;
            dbPerson.Gender = newGender;
            db.Store(dbPerson);
            return true;
        }

        public bool DeletePerson(Person person)
        {
            try
            {
                if (person != null)
                {
                    foreach (var listInclude in GetListsWithTree(person))
                    {
                        var element = listInclude.Children.FirstOrDefault(x => x.child.Name == person.Name);
                        listInclude.Children.Remove(element);
                        db.Delete(element);
                        db.Store(listInclude);
                    }
                    foreach (var el in person.Children.ToList())
                    {
                        DeleteChildFromList(person, el);
                    }
                    db.Delete(person);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIfChildExists(Person person, Child child)
        {
            var childList = DownloadList(person.Name);
            if (childList.Children.Any(x => x.child.Name == child.child.Name))
            {
                return false;
            }
            return true;
        }
        public bool AddChild(Person person, Child child)
        {
            try
            {
                if (CheckCycles(person, child))
                    return false;
                var dbPerson = GetPerson<Person>().FirstOrDefault(x => x.Name == child.child.Name);
                if (person == null || dbPerson == null)
                {
                    return false;
                }

                if (person.Gender == Gender.Kobieta.ToString())
                {
                    child.child.Mother = person.Name;
                    dbPerson.Mother = person.Name;
                }
                else if (person.Gender == Gender.Mezczyzna.ToString())
                {
                    child.child.Father = person.Name;
                    dbPerson.Father = person.Name;
                }

                db.Store(dbPerson);

                person.Children.Add(child);
                db.Store(person);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckParentAge(Person person, Child child, Gender gender)
        {
            if (person.BirthDate != "01/01/0001")
            {
                var parentBirthDate = DateTime.Parse(person.BirthDate);
                var childBirthDate = DateTime.Parse(child.child.BirthDate);
                int ageWhenChildBorn = childBirthDate.Year - parentBirthDate.Year;

                if (gender == Gender.Kobieta)
                {
                    if (ageWhenChildBorn >= 10 && ageWhenChildBorn <= 60)
                        return true;
                    else
                        return false;
                }
                else if (gender == Gender.Mezczyzna)
                {
                    if (ageWhenChildBorn >= 12 && ageWhenChildBorn <= 70)
                        return true;
                    else
                        return false;
                }
                
            }
            return true;
        }

        public bool CheckIfPossibleToBeBorn(Person person, Child child, Gender gender)
        {
            if (person.DeathDate != "01/01/0001")
            {
                var parentDeathDate = DateTime.Parse(person.DeathDate);
                var childBirthDate = DateTime.Parse(child.child.BirthDate);
                int days = (parentDeathDate - childBirthDate).Days;

                if (gender == Gender.Kobieta)
                {
                    if (days >= 0)
                        return true;
                    else
                        return false;
                }
                else if (gender == Gender.Mezczyzna)
                {
                    if (days > -270)
                        return true;
                    else
                        return false;
                }
            }
            return true;
        }

        public bool CheckChildParents(Person person, Child child)
        {
            if (!string.IsNullOrEmpty(child.child.Mother))
            {
                if (person.Gender == Gender.Kobieta.ToString())
                    return false;
                else
                    return true;
            }
            else if (!string.IsNullOrEmpty(child.child.Father))
            {
                if (person.Gender == Gender.Mezczyzna.ToString())
                    return false;
                else
                    return true;
            }
            return true;
        }

        public bool CheckCycles(Person person, Child child)
        {
            var childList = DownloadList(child.child.Name);

            if (person.Name == child.child.Name)
                return true;

            if (childList.Children.Count == 0)
                return false;

            if (childList.Children.Any(x => x.child.Name == person.Name))
                return true;

            foreach (var kid in childList.Children)
            {
                if (CheckCycles(person, kid))
                    return true;
            }
            return false;
        }

        public Person DownloadList(string name)
        {
            return GetPerson<Person>().FirstOrDefault(x => x.Name == name);
        }

        public bool DeleteChildFromList(Person person, Child child)
        {
            try
            {
                var elem = person.Children.FirstOrDefault(x => x.child.Name == child.child.Name);
                if (elem != null)
                {
                    person.Children.Remove(elem);
                    if (person.Gender == Gender.Kobieta.ToString())
                    {
                        child.child.Mother = null;
                    }
                    else if (person.Gender == Gender.Mezczyzna.ToString())
                    {
                        child.child.Father = null;
                    }

                    db.Delete(elem);
                }
                db.Store(person);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<T> GetPerson<T>()
        {
            return db.Query<T>(typeof(T)).ToList();
        }

        public List<Person> GetListsWithTree(Person person)
        {
            return GetPerson<Person>().Where(x => x.Children.Select(y => y.child.Name).Contains(person.Name)).ToList();
        }
    }
}
