using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using ApiMedalkin.Models;

namespace ApiMedalkin.Repository;

public interface IUserRepository
{
    Task<UserModel> GetUserMedalAsync(string reward);
    Task<IEnumerable<UserModel>> GetUserMedalsByRewardAsync(string userName, string chatId);
    Task<bool> DeleteUserMedalAsync(string reward);
    Task AddUserMedalAsync(UserModel userModel);
}
public class UserRepository : IUserRepository
{
    private readonly Table _MedalkinDbTable;
    private readonly IDynamoDBContext _DbContext;
    public UserRepository(IDynamoDBContext dBContext)
    {
        _DbContext = dBContext;
        _MedalkinDbTable = _DbContext.GetTargetTable<UserModel>();
    }

    public async Task AddUserMedalAsync(UserModel userModel)
    {
        var userAsDocument = _DbContext.ToDocument(userModel);

        await _MedalkinDbTable.PutItemAsync(userAsDocument);
    }

    public async Task<bool> DeleteUserMedalAsync(string reward)
    {
        var userModelAsDocument = await _MedalkinDbTable.GetItemAsync(new Primitive(reward));

        if (_DbContext.FromDocument<UserModel>(userModelAsDocument) == null)
        {
            return false;
        }

        await _MedalkinDbTable.DeleteItemAsync(userModelAsDocument);

        return true;
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
