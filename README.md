Orleans.Owin
============

Microsoft Project Orleans initialization middleware for OWIN.

Usage is easy, simply insert the Orleans middleware into your OWIN pipeline using the `ConfigureOrleans()` extension method attached to `IAppBuilder`, in the `Orleans.Owin` namespace.

```C#
public class Startup
{
  public void Configuration(IAppBuilder app)
  {
      app.ConfigureOrleans();
  }
}
```
There are overloads for explicitly providing configuration, otherwise configuration will be sourced from the standard `ClientConfiguration.xml` file.
