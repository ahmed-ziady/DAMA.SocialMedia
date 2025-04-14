public class PostDto
{
    public int PostId { get; set; }
    public string PostContent { get; set; } = null!;
    public int PostTypeId { get; set; }
    public DateTime PostDate { get; set; }
}
