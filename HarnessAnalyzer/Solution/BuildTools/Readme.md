# Usage of vectorDraw machine authorization tools

A machine must be authorized to a serial before it can be used to build a solution with full licensed/valid vectorDraw-dlls.

## vdAuthorize/vdLic:

```
vcLic.exe au [>=VD11 Serial]
```
```
vcAuthorizeApp.exe au [<= VD10 Serial]
```

## UnAuthorize:
```
vcLic.exe unau [>=VD11 Serial]
```
```
vcAuthorizeApp.exe unau [<= VD10 Serial]
```

# Batch files

This are helper scripts to ensure a correct (un-)authorization of a machine for the vectorDraw licensing system without hitting the brutforce check of the licensing server.

The parameter "/ELEV" can be use to avoid checking for elevation and re-running the script under higher rights from the start

```
Authorize.bat [SERIAL] /ELEV (optional)
```
```
UnAuthorize.bat [SERIAL] /ELEV (optional)
```