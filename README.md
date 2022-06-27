# peaPacker

PeaPacker is an open-source image channel packer, still heavily a work in progress.  It's built with C# in Visual Studio.  I'm currently only targeting Windows, but hoping to add Mac and Linux support in the future. I made this to have a quick, stand-alone tool to address my [channel packing](http://wiki.polycount.com/wiki/ChannelPacking) needs without having to fire up bulkier image-editing software.  This is my first Windows Forms application.

## Target Features

- Load in an image
- View split RGBA channels
- Replace individual channels with user-selected images
- Ability to invert channels or fill them with a solid color
- Save final packed image
- Standalone application, no installation necessary, all contained in one .exe