# ExplorerGenie

![Icon](screenshots/explorergenie.png)

ExplorerGenie is an extended context menu for the Windows explorer. It will consist of several
tools to improve working with the Windows explorer, like path copying, opening of the command
line or verification of a file hash.

In a first step, ExplorerGenie will replace the tool [CopyPathMenu](https://www.martinstoeckli.ch/copypathmenu)
and make it open source.

## Security

Because all context menus of the explorer run inside the explorer process (shell-extension), and
therefore can potentially interfere with this process, we took special care about the security and
stability of this extension.

* The shell extension runs isolated from the rest of the application, and its code it kept to a minimum.
* The shell extension is written in Delphi, which compiles to native code. This way we can avoid loading of the DotNet runtime, do not impact performance of explorer and file-open dialogs, and cannot run into problems with incompatible versions.

## Credits

* We got a free version of the [Advanced Installer](https://www.advancedinstaller.com), which simplifies the distribution of this open source app tremendous.
