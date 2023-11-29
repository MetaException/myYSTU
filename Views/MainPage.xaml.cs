﻿using myYSTU.Parsers;

namespace myYSTU.Views;

public partial class MainPage : ContentPage
{
    private Model.Person person;

    public MainPage()
    {
        InitializeComponent();
        ParseAsync();
    }

    private async Task ParseAsync()
    {
        person = await PersonParser.ParseInfo();

        Fullname.Text = person.Name[..person.Name.LastIndexOf(' ')];
        GroupName.Text = person.Group;

        avatar.Source = await PersonParser.ParseAvatar(person.AvatarUrl);
    }

    private void ProfileInfo_Tapped(object sender, TappedEventArgs e)
    {
        InfoBottomSheet sheet = new InfoBottomSheet(); //Пока автор библиотеки не пофиксит ошибку
        sheet.HasBackdrop = true;
        //TODO: переделать
        sheet.setInfo(person);
        sheet.ShowAsync();
    }

    private void EnterToGradesPageButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new GradesPage());
    }

    private void EnterToTimeTablePageButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new TimeTablePage());
    }

    private void EnterToStaffPageButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new StaffPage());
    }
}