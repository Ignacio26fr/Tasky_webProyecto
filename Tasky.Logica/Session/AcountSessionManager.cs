using Microsoft.EntityFrameworkCore;
using Tasky.Datos.EF;


namespace Tasky.Logica.Session;

public interface IAcountSessionManager
{
    Task<GoogleSession> RegisterSession(AspNetUsers user, string accessToken);
    Task<GoogleSession?> GetSession();
    Task RemoveSession();
}

public class AcountSessionManager: IAcountSessionManager
{
    private TaskyContext _taskyContext;

    public AcountSessionManager(TaskyContext taskyContext)
    {
        _taskyContext = taskyContext;
    }

    public async Task<GoogleSession> RegisterSession(AspNetUsers user, string accessToken)
    {
        var session = new GoogleSession
        {
            UserId = user.Id,
            AccessToken = accessToken,
        };

        _taskyContext.googleSessions.Add(session);
        await _taskyContext.SaveChangesAsync();
        return session;

    }

    public async Task<GoogleSession?> GetSession()
    {
        var sessions = await _taskyContext.googleSessions.Include(s => s.User).ToListAsync();

        if (sessions.Count == 0)
            return null;

        return sessions.First();
    }

    public async Task RemoveSession()
    {
        var session = await GetSession();
        _taskyContext.googleSessions.Remove(session);
        await _taskyContext.SaveChangesAsync();
    }
}