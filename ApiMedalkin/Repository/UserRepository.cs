using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using ApiMedalkin.Models;

namespace ApiMedalkin.Repository;

public interface IUserRepository
{
    Task<UserModel> GetUserMedalAsync(string reward);
    Task<IEnumerable<UserModel>> GetUserMedalsByRewardAsync(string userName, string chatId);
    Task DeleteUserMedalAsync(string reward);
    Task AddUserMedalAsync(UserModel userModel);
}
public class UserRepository : IUserRepository
{
    private readonly Table _MedalkinDbTable;
    private readonly IDynamoDBContext _DbContext;
    public UserRepository(IDynamoDBContext dBContext)
    {
        _MedalkinDbTable = _DbContext.GetTargetTable<UserModel>();
        _DbContext = dBContext;
    }

    public async Task AddUserMedalAsync(UserModel userModel)
    {
        var userAsDocument = _DbContext.ToDocument(userModel);

        await _MedalkinDbTable.PutItemAsync(userAsDocument);
    }

    public async Task DeleteUserMedalAsync(string reward)
    {
        var userModelAsDocument = await _MedalkinDbTable.GetItemAsync(new Primitive(reward));

        await _MedalkinDbTable.DeleteItemAsync(userModelAsDocument);
    }

    public async Task<UserModel> GetUserMedalAsync(string reward)
    {
        var responseAsDocument = await _MedalkinDbTable.GetItemAsync(new Primitive(reward));

        return _DbContext.FromDocument<UserModel>(responseAsDocument);
    }

    public async Task<IEnumerable<UserModel>> GetUserMedalsByRewardAsync(string userName, string chatId)
    {
        List<ScanCondition> conditionsToScan = new List<ScanCondition>();
        conditionsToScan.Add(new ScanCondition("UserName", ScanOperator.Contains, userName.Trim('@')));
        conditionsToScan.Add(new ScanCondition("ChatId", ScanOperator.Equal, $"{chatId}"));

        var result = _DbContext.ScanAsync<UserModel>(conditionsToScan);
        return await result.GetRemainingAsync();
    }
}
