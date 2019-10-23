# Kiosk_Keyboard
Windows keyboard for kiosk mode include WebApp compatibility

This keyboard was build to use in app in "Kiosk Mode"

Download the code and compile or use the installer ==> https://github.com/jczunigae/Kiosk_Keyboard/raw/master/Keyboard_Installer/Release/Keyboard_Installer.msi

## Usage

- Once you install the app, can call the keyboard in two ways

1. Http:

The installer add a protocol call it "tkb:"
```
tkb:showPrincipal    ===> call the principal keyboard
tkb:minimize         ===> minimize keyboard
tkb:showNumbers      ===> call just numbers keyboard
```
2. Telnet:

Once the keyboard is running, the app is listening a port waiting for a command (port 2000 by default, you can change the port at the app.config file of the instalation)
```
$ telnet 127.0.0.1 2000
```
commands: 
```
showPrincipal    ===> call the principal keyboard
minimize         ===> minimize keyboard
showNumbers      ===> call just numbers keyboard
```
3. Bonus (You can call Http with Javascript integration)
```
function callKeyboard(keyboardType){
    //New function to call keyboard!!
	  console.log("New function to call keyboard :: " + keyboardType)
    var ifrm = document.createElement("iframe");
    ifrm.setAttribute("src", keyboardType);// 'tkb:showNumbers', 'tkb:showPrincipal', 'tkb:minimize'
    ifrm.style.display = "none";
    document.body.appendChild(ifrm);
}
```
### Example page
You must install keyboard before test the page

http://automateit.mx/keyboard/example.html

<img src="http://automateit.mx/keyboard/keyboardExample.gif"/>

## Configuration

- Config file (app.config)
You can change language and port in the configuration file of the installation
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5"/>
  </startup>
  <appSettings>
    <!-- Port for telnet connection -->
    <add key="TcpPort" value="2000"/>
    <add key="ClientSettingsProvider.ServiceUri" value=""/>
    <!--Available languages: English:EN, Spanish: ES-->
    <add key="KeyboardLanguage" value="EN"/>
    <!--If false show onlye the width of the keyboard without fill all the space with gray -->
    <add key="ShowBackground" value="true"/>
  </appSettings>
  <system.web>
    <membership defaultProvider="ClientAuthenticationMembershipProvider">
      <providers>
        <add name="ClientAuthenticationMembershipProvider" type="System.Web.ClientServices.Providers.ClientFormsAuthenticationMembershipProvider, System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" serviceUri=""/>
      </providers>
    </membership>
    <roleManager defaultProvider="ClientRoleProvider" enabled="true">
      <providers>
        <add name="ClientRoleProvider" type="System.Web.ClientServices.Providers.ClientRoleProvider, System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" serviceUri="" cacheTimeout="86400"/>
      </providers>
    </roleManager>
  </system.web>
</configuration>
```
