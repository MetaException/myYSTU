namespace myYSTU.Models;

public class Staff : IAvatarModel
{
    public string Name { get; set; }
    public string Post { get; set; }
    public ImageSource AvatarImageSource { get; set; }
    public string AvatarUrl { get; set; }
}
