﻿using myYSTU.ViewModels;

namespace myYSTU.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}