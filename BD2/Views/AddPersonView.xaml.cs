﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BD2.ViewModels;

namespace BD2.Views
{

    /// <summary>
    /// Interaction logic for AddList.xaml
    /// </summary>
    public partial class AddPersonView : Window
    {
        public enum Gender
        {
            Kobieta,
            Mezczyzna
        }

        public AddPersonView()
        {
            InitializeComponent();
        }
    }
}
