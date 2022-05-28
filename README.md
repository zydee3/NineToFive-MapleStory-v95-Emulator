# NineToFive
v95 MapleStory server emulator in C#

This was a summer project between [Izarooni](https://github.com/izarooni/) and myself.

# Information about this project:
- The server was written modularly such that all services could run on independent machines. 
- Packet structures and naming conventions were heavily inspired by nexon's distributed v95 IDB. 
- Wz data was loaded directly from the installation files using MapleLib. 
- Progression into the project was as far as handling necessary packets (movement, entity spawns, entity interaction, attacks, inventories, and some keymaps).
- Other things such as summons and pets may be working, but needs to be confirmed. 

Last I recall I sent the wrong packet for keybinds. Attached is the last saved screen shot of the packet structure I have regarding this issue so maybe it will be useful to whoever may continue this project.

![unknown](https://user-images.githubusercontent.com/26658058/168234364-d09ea6f5-47c5-480f-9960-b09cf520a703.png)

# General Screenshots:

![8c9040e55ba545c441f8b8eeb86cbf98](https://user-images.githubusercontent.com/26658058/168232896-91aabace-8f78-4686-b3c0-68d2d5cd26d5.png)

![b15f90c20a7a91b7dbc86868a9cc8a21](https://user-images.githubusercontent.com/26658058/168234837-338b1ffa-81c7-4ad3-9309-1adf145bfde3.png)

https://user-images.githubusercontent.com/26658058/168230235-086b7953-9be1-476f-aae1-3fb7ade894d0.mp4
