using Amazon.DynamoDBv2.DataModel;

namespace ApiMedalkin.Models;

[DynamoDBTable("MedalkinDb")]
public class UserModel
{
    [DynamoDBProperty("UserName")]
    public string UserName { get; set; }

    [DynamoDBProperty("ChatId")]
    public string ChatId { get; set; }

    [DynamoDBHashKey("Reward")]
    public string Reward { get; set; }

    [DynamoDBProperty("Emoji")]
    public string Emoji { get; set; }

    [DynamoDBProperty("Date")]
    public DateTime Date { get; set; }

    [DynamoDBProperty("Description")]
    public string Description { get; set; }
}
