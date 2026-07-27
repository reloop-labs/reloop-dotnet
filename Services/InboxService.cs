namespace Reloop.Services;

/** Inbox mailboxes, messages, and threads. */
public class InboxService
{
    public InboxMailboxesService Mailboxes { get; }
    public InboxMessagesService Messages { get; }
    public InboxThreadsService Threads { get; }

    internal InboxService(ReloopClient client)
    {
        Mailboxes = new InboxMailboxesService(client);
        Messages = new InboxMessagesService(client);
        Threads = new InboxThreadsService(client);
    }
}
