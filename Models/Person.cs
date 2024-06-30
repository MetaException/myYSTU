using CommunityToolkit.Mvvm.ComponentModel;

namespace myYSTU.Models;

public partial class Person : ObservableObject, IAvatarModel
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Status { get; set; }
    public string Faculty { get; set; }
    public string Group { get; set; }
    public string StudyDirection { get; set; }
    public string SourceOfFunding { get; set; }
    public string Qualification { get; set; }
    public string FormOfEducation { get; set; }
    public string DateOfBirth { get; set; }
    public string HomeAddress { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string LibraryDept { get; set; }
    public string MoneyDept { get; set; }
    public string DormitoryDept { get; set; }
    public string MilitaryRec { get; set; }
    public string LibaryCard { get; set; }
    public string Login { get; set; }

    [ObservableProperty]
    private ImageSource _avatarImageSource;
    public string AvatarUrl { get; set; }
}
