Installutil.exe COM.TIGER.SYNC.Service.exe

Net Start COMTIGERDATASYNC

sc config COMTIGERDATASYNC start= auto 