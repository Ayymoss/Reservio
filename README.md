# IW4MNameReserve - Plugin for RaidMax's IW4MAdmin

Name Reserve will reserve a target's name by GUID, so if someone joins with a reserved name that's not tied to the GUID they will be kicked from the game with the reason to change their name.

To reserve a new name/client see below command, you can update or add a new client by running it on the same user with a new name.

***

## Commands:
```
!reserve (!rsrv) <target> 
```

Note, the configuration will only save/write when you `!quit` out of IW4MAdmin, so don't modify the config whilst IW4MAdmin is open else you will lose the changes.

Configuration is in `Configuration/ReservedClientsSettings.json`
