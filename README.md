install Unity version: 2018.3.14f1: there is an option to install visual studio that belongs to it
compile everything and try to run it, ran fine on macbook pro

check for relative paths to Unity installation and to Library, why are some paths to library and some to unity installation?

setup visual studio for unity, compile code in visual studio


flow Unity player switches weapon

PlayerHand.cs: NetworkWeaponChanged calls LocalPlayerChangedToWeapon

in Client.cs LocalPlayerChangedToWeapon: send Buffer


NetworkPlayer needs to react on it somehow

NetworkPlayer.cs in Update(): latestPlayerData = client.GetLatestDatas(networkId); in    if (latestPlayerData.appereancesChanged) LoadAppereances();
        if (latestPlayerData.weaponChanged) EquipWeapon();

the weapon switch is made visible to all the players: EquipWeapon() has Unity functions to equip the weapon. 


since the Update() routine checks for 
latestPlayerData.weaponChanged and calls EquipWeapon, the only part that is missing is:
a server that takes the sent data from all the clients and sets the values in latestPlayerData hashtable. the Update() call will distribute the information. 


in Client.cs
Packet() has as input data, creates a buffer off it, checks it and then calls PlayerChangeWeapon PlayerFireWeapon PlayerLeft etc. Inside PlayerFireWeapon there is: ((PlayerData)playerDatas[playerID]).weaponChanged = true;

and this triggers the Update function in NetworkPlayer.cs

This partly solves the paragraph above, only question is when and how is Packet called?

Packet is called in Refresh, Refresh is called in Threadstart and thus called all the time

seems like all I need to do is setup a listener (socket?) which is able to receive and send( the receive and send methods are already implemented in the client). I just need to provide a place(server) where the data can be sent to.


https://stackoverflow.com/questions/57671443/solved-socket-exception-no-connection-could-be-made-because-the-target-machi
