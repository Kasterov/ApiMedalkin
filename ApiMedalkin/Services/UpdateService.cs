using ApiMedalkin.Models;
using ApiMedalkin.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ApiMedalkin.Services;

public interface IUpdateService
{
    Task HandleUpdate(Update update, CancellationToken cancellationToken = default);
}
public class UpdateService : IUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserRepository _userRepository;

    public UpdateService(ITelegramBotClient botClient,
        IUserRepository userRepository)
    {
        _botClient = botClient;
        _userRepository = userRepository;
    }

    public async Task HandleUpdate(Update update, CancellationToken cancellationToken = default)
    {
        var message = update.Message;

        try
        {

            if (message.Text == "/medalkin" || message.Text == "/medalkin@Medalkin_bot")
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, "Привет, я Медалькин! Я умею:\n\n" +
                    "выдать медльку:\n/give_medalka\n@username\nemoji\ndescription\n\nзабрать медальку:\n/take_medalka\n@username\nemoji\n\nпроверить медальки:\n/check_medalki\n@username");
                return;
            }

            if (message.Text.Contains("/give_medalka"))
            {
                try
                {
                    var command = message.Text.Split("\n");

                    if (command[1].Contains(message.From.Username))
                    {
                        await _botClient.SendTextMessageAsync(message.Chat.Id, "Себе медальки выдавать нельзя, Гений!");
                        return;
                    }

                    if (await _userRepository.IsChatHasMedal(command[2], message.Chat.Id.ToString()))
                    {
                        await _botClient.SendTextMessageAsync(message.Chat.Id, $"В данном чате уже имеется юзер с такой медалькой!");
                        return;
                    }

                    var request = new UserModel()
                    {
                        UserName = command[1].Trim('@'),
                        Emoji = command[2],
                        Description = command[3],
                        Reward = $"{command[2]},{command[1].Trim('@')},{message.Chat.Id}",
                        ChatId = message.Chat.Id.ToString(),
                        Date = DateTime.Now.AddHours(2)
                    };

                    await _userRepository.AddUserMedalAsync(request);

                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Готово! Проверить медальки можно по команде:\n\n /check_medalki\n@username");

                    return;
                }

                catch
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, $"Не понял чего вы хотите! :(");
                    return;
                }
            }

            if (message.Text.Contains("/check_medalki"))
            {
                try
                {
                    List<string> command;

                    if (message.Text == "/check_medalki" || message.Text == "/check_medalki@Medalkin_bot")
                    {
                        command = new List<string>() { "0", message.From.Username };
                    }
                    else
                    {
                        command = message.Text.Split("\n").ToList();
                    }

                    var medals = await _userRepository.GetUserMedalsByRewardAsync(command[1], message.Chat.Id.ToString());

                    if (medals.ToList().Count == 0)
                    {
                        await _botClient.SendTextMessageAsync(message.Chat.Id, $"К сожалению у {command[1]} нет медалек!");
                        return;
                    }

                    foreach (var medal in medals)
                    {
                        await _botClient.SendTextMessageAsync(message.Chat.Id, $"{medal.Emoji} - {medal.Description} - {medal.Date}\n\n");
                    }
                }
                catch
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, $"Не понял :(");
                }
            }

            if (message.Text.Contains("/take_medalka"))
            {
                try
                {
                    var command = message.Text.Split("\n");

                    if (command[1].Contains(message.From.Username))
                    {
                        await _botClient.SendTextMessageAsync(message.Chat.Id, "Забирать медальки у себя нельзя (Даже если они очень не нравятся)!");
                        return;
                    }

                    string reward = $"{command[2]},{command[1].Trim('@')},{message.Chat.Id}";

                    if (!await _userRepository.DeleteUserMedalAsync(reward))
                    {
                        await _botClient.SendTextMessageAsync(message.Chat.Id, $"У {command[1]} нет такой медальки!");
                        return;
                    }

                    await _botClient.SendTextMessageAsync(message.Chat.Id, $"Таааакс, я все сделал!");
                    return;
                }

                catch
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, $"Я не понял вашего запроса! :(");
                    return;
                }
            }
        }

        catch (Exception ex)
        {
            //await _botClient.SendTextMessageAsync(message.Chat.Id, $"{ex}\n - Нихуя у вас тут эксепшин!");
        }
    }
}
