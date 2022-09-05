# UnityMvvmToolkit

A package that brings data-binding to your Unity project.

![git-main](https://user-images.githubusercontent.com/28132516/187478087-909fc50b-778b-4827-8090-c5a66d7b6b11.png)

## :open_book: Table of Contents

- [About](#pencil-about)
  - [Samples](#samples)
- [Folder Structure](#cactus-folder-structure)
- [Installation](#gear-installation)
  - [IL2CPP restriction](#il2cpp-restriction)
- [Introduction](#ledger-introduction)
  - [ViewModel](#viewmodel)
  - [CanvasView\<TBindingContext\>](#canvasviewtbindingcontext)
  - [DocumentView\<TBindingContext\>](#documentviewtbindingcontext)
  - [Command & Command\<T\>](#command--commandt)
  - [AsyncCommand & AsyncCommand\<T\>](#asynccommand--asynccommandt)
  - [AsyncLazyCommand & AsyncLazyCommand\<T\>](#asynclazycommand--asynclazycommandt)
  - [PropertyValueConverter\<TSourceType, TTargetType\>](#propertyvalueconvertertsourcetype-ttargettype)
  - [ParameterValueConverter\<TTargetType\>](#parametervalueconverterttargettype)
- [Quick start](#watch-quick-start)
- [How To Use](#rocket-how-to-use)
  - [Data-binding](#data-binding)
  - [Create custom control](#create-custom-control)
- [External Assets](#link-external-assets)
  - [UniTask](#unitask)
- [Benchmarks](#chart_with_upwards_trend-benchmarks)
- [Contributing](#bookmark_tabs-contributing)
  - [Discussions](#discussions)
  - [Report a bug](#report-a-bug)
  - [Request a feature](#request-a-feature)
  - [Show your support](#show-your-support)
- [License](#balance_scale-license)

## :pencil: About

The **UnityMvvmToolkit** is a package that allows you to bind UI elements in your `UI Document` or `Canvas` to data sources in your app. Use the samples as a starting point for understanding how to utilize the package.

Key features:
- Runtime data-binding
- UI Toolkit & uGUI integration
- Multiple-properties binding
- Custom UI Elements support
- Compatible with [UniTask](https://github.com/Cysharp/UniTask)
- Mono & IL2CPP support[*](#il2cpp-restriction)

### Samples

The following example shows the **UnityMvvmToolkit** in action using the **Counter** app.

<details open><summary><b>CounterView</b></summary>
<br />

```xml
<UXML>
    <BindableContentPage binding-theme-mode-path="ThemeMode" class="counter-screen">
        <VisualElement class="number-container">
            <BindableCountLabel binding-text-path="Count" class="count-label count-label--animation" />
        </VisualElement>
        <BindableThemeSwitcher binding-value-path="ThemeMode, Converter={ThemeModeToBoolConverter}" />
        <BindableCounterSlider increment-command="IncrementCommand" decrement-command="DecrementCommand" />
    </BindableContentPage>
</UXML>
```

> **Note:** The namespaces are omitted to make the example more readable.

</details>

<details><summary><b>CounterViewModel</b></summary>
<br />

```csharp
public class CounterViewModel : ViewModel
{
    private int _count;
    private ThemeMode _themeMode;

    public CounterViewModel()
    {
        IncrementCommand = new Command(IncrementCount);
        DecrementCommand = new Command(DecrementCount);
    }

    public int Count
    {
        get => _count;
        set => Set(ref _count, value);
    }

    public ThemeMode ThemeMode
    {
        get => _themeMode;
        set => Set(ref _themeMode, value);
    }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }

    private void IncrementCount() => Count++;
    private void DecrementCount() => Count--;
}
```

</details>

<table>
  <tr>
    <td align="center">Counter</td>
    <td align="center">Calculator</td>
    <td align="center">ToDoList</td>
  </tr>
  <tr>
    <td align="center" width=25%>
      <video src="https://user-images.githubusercontent.com/28132516/187030099-a440bc89-4c28-44e3-9898-9894eac5bff4.mp4" alt="CounterSample" />
    </td>
    <td align="center" width=25%>
      <video src="https://user-images.githubusercontent.com/28132516/187471982-4acdeddb-ec4d-45b6-a2a3-4198a03760de.mp4" alt="CalculatorSample" />
    </td>
    <td align="center" width=25%>
      <video src="https://user-images.githubusercontent.com/28132516/187030101-ad1f2123-59d5-4d1e-a9ca-ab983589e52f.mp4" alt="ToDoListSample" />
    </td>
  </tr>
</table>

> You will find all the samples in the `samples` folder.

## :cactus: Folder Structure

    .
    ├── samples
    │   ├── Unity.Mvvm.Calc
    │   ├── Unity.Mvvm.Counter
    │   ├── Unity.Mvvm.ToDoList
    │   └── Unity.Mvvm.CounterLegacy
    │
    ├── src
    │   ├── UnityMvvmToolkit.Core
    │   └── UnityMvvmToolkit.UnityPackage
    │       ...
    │       ├── Core      # Auto-generated
    │       ├── Common
    │       ├── External
    │       ├── UGUI
    │       └── UITK
    │
    ├── UnityMvvmToolkit.sln

## :gear: Installation

You can install UnityMvvmToolkit in one of the following ways:

<details><summary>1. Install via Package Manager</summary>
<br />
  
  The package is available on the [OpenUPM](https://openupm.com/packages/com.chebanovdd.unitymvvmtoolkit/).

  - Open `Edit/Project Settings/Package Manager`
  - Add a new `Scoped Registry` (or edit the existing OpenUPM entry)

    ```
    Name      package.openupm.com
    URL       https://package.openupm.com
    Scope(s)  com.cysharp.unitask
              com.chebanovdd.unitymvvmtoolkit
    ```
  - Open `Window/Package Manager`
  - Select `My Registries`
  - Install `UniTask` and `UnityMvvmToolkit` packages
  
</details>

<details><summary>2. Install via Git URL</summary>
<br />
  
  You can add `https://github.com/ChebanovDD/UnityMvvmToolkit.git?path=src/UnityMvvmToolkit.UnityPackage/Assets/Plugins/UnityMvvmToolkit` to the Package Manager.

  If you want to set a target version, UnityMvvmToolkit uses the `v*.*.*` release tag, so you can specify a version like `#v0.1.0`. For example `https://github.com/ChebanovDD/UnityMvvmToolkit.git?path=src/UnityMvvmToolkit.UnityPackage/Assets/Plugins/UnityMvvmToolkit#v0.1.0`.
  
</details>

### IL2CPP restriction

The **UnityMvvmToolkit** uses generic virtual methods under the hood to create bindable properties, but `IL2CPP` in `Unity 2021` does not support [Full Generic Sharing](https://blog.unity.com/technology/feature-preview-il2cpp-full-generic-sharing-in-unity-20221-beta) this restriction will be removed in `Unity 2022`.

To work around this issue in `Unity 2021` you need to change the `IL2CPP Code Generation` setting in the `Build Settings` window to `Faster (smaller) builds`.

<details><summary>Instruction</summary>
<br />

![build-settings](https://user-images.githubusercontent.com/28132516/187468236-4b455b62-48ef-4e9c-9a3f-83391833c3c0.png)

</details>

## :ledger: Introduction

The package contains a collection of standard, self-contained, lightweight types that provide a starting implementation for building apps using the MVVM pattern.

The included types are:
- [ViewModel](#viewmodel)
- [CanvasView\<TBindingContext\>](#canvasviewtbindingcontext)
- [DocumentView\<TBindingContext\>](#documentviewtbindingcontext)
- [Command & Command\<T\>](#command--commandt)
- [AsyncCommand & AsyncCommand\<T\>](#asynccommand--asynccommandt)
- [AsyncLazyCommand & AsyncLazyCommand\<T\>](#asynclazycommand--asynclazycommandt)
- [PropertyValueConverter\<TSourceType, TTargetType\>](#propertyvalueconvertertsourcetype-ttargettype)
- [ParameterValueConverter\<TTargetType\>](#parametervalueconverterttargettype)
- [ICommand & ICommand\<T\>](#command--commandt)
- [IAsyncCommand & IAsyncCommand\<T\>](#asynccommand--asynccommandt)
- [ICommandWrapper](#commandwrapper)
- [IPropertyValueConverter\<TSourceType, TTargetType\>](#propertyvalueconvertertsourcetype-ttargettype)
- [IParameterValueConverter\<TTargetType\>](#parametervalueconverterttargettype)

### ViewModel

The `ViewModel` is a base class for objects that are observable by implementing the `INotifyPropertyChanged` interface. It can be used as a starting point for all kinds of objects that need to support property change notification.

Key functionality:
- Provides a base implementation for `INotifyPropertyChanged`, exposing the `PropertyChanged` events
- Provides a series of `Set` methods that can be used to easily set property values from types inheriting from `ViewModel`, and to automatically raise the appropriate events

> **Note:** In case your viewmodel doesn't have a parameterless constructor, you need to override the `GetBindingContext` method on the view.

#### Simple property

Here's an example of how to implement notification support to a custom property.

```csharp
public class CounterViewModel : ViewModel
{
    private int _count;

    public int Count
    {
        get => _count;
        set => Set(ref _count, value);
    }
}
```

The provided `Set<T>(ref T, T, string)` method checks the current value of the property, and updates it if different, and then also raises the `PropertyChanged` event automatically. The property name is automatically captured through the use of the `[CallerMemberName]` attribute, so there's no need to manually specify which property is being updated.

#### Wrapping a model

To inject notification support to models, that don't implement the `INotifyPropertyChanged` interface, `ViewModel` provides a dedicated `Set<TModel, T>(T, T, TModel, Action<TModel, T>, string)` method for this.

```csharp
public class UserViewModel : ViewModel
{
    private readonly User _user;

    public UserViewModel(User user)
    {
        _user = user;
    }

    public string Name
    {
        get => _user.Name;
        set => Set(_user.Name, value, _user, (user, name) => user.Name = name);
    }
}
```

### CanvasView\<TBindingContext\>

The `CanvasView<TBindingContext>` is a base class for `uGUI` view's.

Key functionality:
- Provides a base implementation for `Canvas` based view
- Automatically searches for bindable UI elements on the `Canvas`
- Allows to override the base viewmodel instance creation
- Allows to define 'property' & 'parameter' value converters
- Allows to define custom UI elements

```csharp
public class CounterView : CanvasView<CounterViewModel>
{
    // Override the base viewmodel instance creation.
    // Required in case there is no default constructor for the viewmodel.
    protected override CounterViewModel GetBindingContext()
    {
        return _appContext.Resolve<CounterViewModel>();
    }

    // Define 'property' & 'parameter' value converters.
    protected override IValueConverter[] GetValueConverters()
    {
        return _appContext.Resolve<IValueConverter[]>();
    }

    // Define custom UI elements.
    protected override IBindableElementsFactory GetBindableElementsFactory()
    {
        return _appContext.Resolve<IBindableElementsFactory>();
    }
}
```

### DocumentView\<TBindingContext\>

The `DocumentView<TBindingContext>` is a base class for `UI Toolkit` view's.

Key functionality:
- Provides a base implementation for `UI Document` based view
- Automatically searches for bindable UI elements on the `UI Document`
- Allows to override the base viewmodel instance creation
- Allows to define 'property' & 'parameter' value converters
- Allows to define custom UI elements

```csharp
public class CounterView : DocumentView<CounterViewModel>
{
    // Override the base viewmodel instance creation.
    // Required in case there is no default constructor for the viewmodel.
    protected override CounterViewModel GetBindingContext()
    {
        return _appContext.Resolve<CounterViewModel>();
    }

    // Define 'property' & 'parameter' value converters.
    protected override IValueConverter[] GetValueConverters()
    {
        return _appContext.Resolve<IValueConverter[]>();
    }

    // Define custom UI elements.
    protected override IBindableElementsFactory GetBindableElementsFactory()
    {
        return _appContext.Resolve<IBindableElementFactory>();
    }
}
```

### Command & Command\<T\>

The `Command` and `Command<T>` are `ICommand` implementations that can expose a method or delegate to the view. These types act as a way to bind commands between the viewmodel and UI elements.

Key functionality:
- Provide a base implementation of the `ICommand` interface
- Implement the `ICommand` & `ICommand<T>` interface, which exposes a `RaiseCanExecuteChanged` method to raise the `CanExecuteChanged` event
- Expose constructor taking delegates like `Action` and `Action<T>`, which allow the wrapping of standard methods and lambda expressions

The following shows how to set up a simple command.

```csharp
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

public class CounterViewModel : ViewModel
{
    private int _count;

    public CounterViewModel()
    {
        IncrementCommand = new Command(IncrementCount);
    }

    public int Count
    {
        get => _count;
        set => Set(ref _count, value);
    }

    public ICommand IncrementCommand { get; }

    private void IncrementCount() => Count++;
}
```

And the relative UI could then be (using UXML).

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <uitk:BindableLabel binding-text-path="Count" />
    <uitk:BindableButton command="IncrementCommand" />
</ui:UXML>
```

The `BindableButton` binds to the `ICommand` in the viewmodel, which wraps the private `IncrementCount` method. The `BindableLabel` displays the value of the `Count` property and is updated every time the property value changes.

> **Note:** You need to define `IntToStrConverter` to convert int to string.

### AsyncCommand & AsyncCommand\<T\>

The `AsyncCommand` and `AsyncCommand<T>` are `ICommand` implementations that extend the functionalities offered by `Command`, with support for asynchronous operations.

Key functionality:
- Extend the functionalities of the synchronous commands included in the package, with support for UniTask-returning delegates
- Can wrap asynchronous functions with a `CancellationToken` parameter to support cancelation, and they expose a `DisableOnExecution` property, as well as a `Cancel` method
- Implement the `IAsyncCommand` & `IAsyncCommand<T>` interfaces, which allows to replace a command with a custom implementation, if needed

Let's say we want to download an image from the web and display it as soon as it downloads.

```csharp
public class ImageViewerViewModel : ViewModel
{
    private readonly IImageDownloader _imageDownloader;
    private Texture2D _texture;

    public ImageViewerViewModel(IImageDownloader imageDownloader)
    {
        _imageDownloader = imageDownloader;
        DownloadImageCommand = new AsyncCommand(DownloadImageAsync);
    }

    public Texture2D Image
    {
        get => _texture;
        private set => Set(ref _texture, value);
    }

    public IAsyncCommand DownloadImageCommand { get; }

    private async UniTask DownloadImageAsync(CancellationToken cancellationToken = default)
    {
        Image = await _imageDownloader.DownloadRandomImageAsync(cancellationToken);
    }
}
```

With the related UI code.

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <BindableImage binding-image-path="Image" />
    <uitk:BindableButton command="DownloadImageCommand">
        <ui:Label text="Download Image" />
    </uitk:BindableButton>
</ui:UXML>
```

To disable the `BindableButton` while an async operation is running, simply set the `DisableOnExecution` property of the `AsyncCommand` to `true`.

```csharp
public class ImageViewerViewModel : ViewModel
{
    public ImageViewerViewModel(IImageDownloader imageDownloader)
    {
        ...
        DownloadImageCommand = new AsyncCommand(DownloadImageAsync) { DisableOnExecution = true };
    }
}
```

If you want to create an async command that supports cancellation, use the `WithCancellation` extension method.

```csharp
public class MyViewModel : ViewModel
{
    public MyViewModel()
    {
        MyAsyncCommand = new AsyncCommand(DoSomethingAsync).WithCancellation();
        CancelCommand = new Command(Cancel);
    }

    public IAsyncCommand MyAsyncCommand { get; }
    public ICommand CancelCommand { get; }
    
    private void Cancel()
    {
        // If the underlying command is not running, or
        // if it does not support cancellation, this method will perform no action.
        MyAsyncCommand.Cancel();
    }
}
```

If the command supports cancellation, previous invocations will automatically be canceled if a new one is started.

> **Note:** You need to import the [UniTask](https://github.com/Cysharp/UniTask) package in order to use async commands.

### AsyncLazyCommand & AsyncLazyCommand\<T\>

The `AsyncLazyCommand` and `AsyncLazyCommand<T>` are have the same functionality as the `AsyncCommand`'s, except they prevent the same async command from being invoked concurrently multiple times.

Let's imagine a scenario similar to the one described in the `AsyncCommand` sample, but a user clicks the `Download Image` button several times while the async operation is running. In this case, `AsyncLazyCommand` will ignore all clicks until previous async operation has completed.

> **Note:** You need to import the [UniTask](https://github.com/Cysharp/UniTask) package in order to use async commands.

### CommandWrapper

...

### PropertyValueConverter\<TSourceType, TTargetType\>

Property value converters provide a way to apply custom logic to a property binding.

Built-in property value converters:
- IntToStrConverter
- FloatToStrConverter

If you want to create your own property value converter, create a class that inherits the `PropertyValueConverter<TSourceType, TTargetType>` abstract class and then implement the `Convert` and `ConvertBack` methods.

```csharp
public enum ThemeMode
{
    Light = 0,
    Dark = 1
}

public class ThemeModeToBoolConverter : PropertyValueConverter<ThemeMode, bool>
{
    // From source to target. 
    public override bool Convert(ThemeMode value)
    {
        return (int) value == 1;
    }

    // From target to source.
    public override ThemeMode ConvertBack(bool value)
    {
        return (ThemeMode) (value ? 1 : 0);
    }
}
```
Don't forget to register the `ThemeModeToBoolConverter` in the view.

```csharp
public class MyView : DocumentView<MyViewModel>
{
    protected override IValueConverter[] GetValueConverters()
    {
        return new IValueConverter[] { new ThemeModeToBoolConverter() };
    }
}
```

Then you can use the `ThemeModeToBoolConverter` as in the following example.

```xml
<UXML>
    <MyBindableElement binding-value-path="ThemeMode, Converter={ThemeModeToBoolConverter}" />
    <!--or-->
    <MyBindableElement binding-value-path="ThemeMode, ThemeModeToBoolConverter" />
</UXML>
```

### ParameterValueConverter\<TTargetType\>

Parameter value converters allow to convert a command parameter.

Built-in parameter value converters:
- ParameterToStrConverter
- ParameterToIntConverter
- ParameterToFloatConverter

By default, the converter is not needed if your command has a `ReadOnlyMemory<char>` parameter type.

```csharp
public class MyViewModel : ViewModel
{
    public MyViewModel()
    {
        PrintParameterCommand = new Command<ReadOnlyMemory<char>>(PrintParameter);
    }

    public ICommand<ReadOnlyMemory<char>> PrintParameterCommand { get; }

    private void PrintParameter(ReadOnlyMemory<char> parameter)
    {
        Debug.Log(parameter);
    }
}
```

```xml
<UXML>
    <BindableButton command="PrintParameterCommand, Parameter={MyParameter}" />
    <!--or-->
    <BindableButton command="PrintParameterCommand, MyParameter" />
</UXML>
```

If you want to create your own parameter value converter, create a class that inherits the `ParameterValueConverter<TTargetType>` abstract class and then implement the `Convert` method.

```csharp
public class ParameterToIntConverter : ParameterValueConverter<int>
{
    public override int Convert(ReadOnlyMemory<char> parameter)
    {
        return int.Parse(parameter.Span);
    }
}
```

Don't forget to register the `ParameterToIntConverter` in the view.

```csharp
public class MyView : DocumentView<MyViewModel>
{
    protected override IValueConverter[] GetValueConverters()
    {
        return new IValueConverter[] { new ParameterToIntConverter() };
    }
}
```

Then you can use the `ParameterToIntConverter` as in the following example.

```csharp
public class MyViewModel : ViewModel
{
    public MyViewModel()
    {
        PrintParameterCommand = new Command<int>(PrintParameter);
    }

    public ICommand<int> PrintParameterCommand { get; }

    private void PrintParameter(int parameter)
    {
        Debug.Log(parameter);
    }
}
```

```xml
<UXML>
    <BindableButton command="PrintIntParameterCommand, Parameter={5}, Converter={ParameterToIntConverter}" />
    <!--or-->
    <BindableButton command="PrintIntParameterCommand, 5, ParameterToIntConverter" />
</UXML>
```

## :watch: Quick start

Once the `UnityMVVMToolkit` is installed, create a class `MyFirstViewModel` and inherit from the `ViewModel`.

```csharp
using UnityMvvmToolkit.Core;

public class MyFirstViewModel : ViewModel
{
    private string _text;

    public MyFirstViewModel()
    {
        _text = "Hello World";
    }

    public string Text
    {
        get => _text;
        set => Set(ref _text, value);
    }
}
```

#### UI Toolkit

Create a class `MyFirstDocumentView` and inherit from the `DocumentView<TBindingContext>`.

```csharp
using UnityMvvmToolkit.UITK;

public class MyFirstDocumentView : DocumentView<MyFirstViewModel>
{
}
```

Create a file `MyFirstView.uxml`, add a `BindableLabel` control and set the `binding-text-path` to `Text`.

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <uitk:BindableLabel binding-text-path="Text" />
</ui:UXML>
```

Add `UI Document` to the scene, set the `MyFirstView.uxml` as a `Source Asset` and add the `MyFirstDocumentView` component to it.

<details><summary>UI Document Inspector</summary>
<br />

![ui-document-inspector](https://user-images.githubusercontent.com/28132516/187613060-e20a139d-72fc-4088-b8d5-f9a01f5afa5b.png)

</details>

#### Unity UI (uGUI)

Create a class `MyFirstCanvasView` and inherit from the `CanvasView<TBindingContext>`.

```csharp
using UnityMvvmToolkit.UGUI;

public class MyFirstCanvasView : CanvasView<MyFirstViewModel>
{
}
```

Add `Canvas` to the scene, and add the `MyFirstCanvasView` component to it.

<details><summary>Canvas Inspector</summary>
<br />

![canvas-inspector](https://user-images.githubusercontent.com/28132516/187613633-2c61c82e-ac25-4319-8e8d-1954eb4be197.png)

</details>

Add a `Text - TextMeshPro` UI element to the canvas, add the `BindableLabel` component to it and set the `BindingTextPath` to `Text`.

<details><summary>Canvas Text Inspector</summary>
<br />

![canvas-text-inspector](https://user-images.githubusercontent.com/28132516/187614103-ad42d000-b3b7-4265-96a6-f6d4db6e8978.png)

</details>

## :rocket: How To Use

### Data-binding

The package contains a set of standard bindable UI elements out of the box.

The included UI elements are:
- [BindableLabel](bindablelabel)
- [BindableTextField](bindabletextfield)
- [BindableButton](bindablebutton)
- [BindableListView](bindablelistview)
- [BindableScrollView](bindablescrollview)

> **Note:** The `ListView` & `ScrollView` are provided for `UI Toolkit` only.

#### BindableLabel

`OneWay` binding

#### BindableTextField

`TwoWay` binding

#### BindableButton

...

#### BindableListView

...

#### BindableScrollView

...

### Create custom control

```csharp
public class Image : VisualElement
{
    public void SetImage(Texture2D image)
    {
        // To prevent memory leaks.
        style.backgroundImage.Release(); // Object.Destroy(background.value.texture);
        style.backgroundImage = new StyleBackground(image);
    }

    public new class UxmlFactory : UxmlFactory<Image, UxmlTraits> {}
    
    public new class UxmlTraits : VisualElement.UxmlTraits {}
}
```

```csharp
public class BindableImage : Image, IBindableUIElement
{
    public string BindingImagePath { get; set; }

    public new class UxmlFactory : UxmlFactory<BindableImage, UxmlTraits> {}

    public new class UxmlTraits : Image.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription _bindingImageAttribute = new()
            { name = "binding-image-path", defaultValue = "" };

        public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
        {
            base.Init(visualElement, bag, context);
            ((BindableImage) visualElement).BindingImagePath = _bindingImageAttribute.GetValueFromBag(bag, context);
        }
    }
}
```

```csharp
public class BindableImageWrapper : BindablePropertyElement
{
    private readonly BindableImage _bindableImage;
    private readonly IReadOnlyProperty<Texture2D> _imageProperty;

    public BindableImageWrapper(BindableImage bindableImage, IObjectProvider objectProvider) : base(objectProvider)
    {
        _bindableImage = bindableImage;
        _imageProperty = GetReadOnlyProperty<Texture2D>(bindableImage.BindingImagePath);
    }

    public override void UpdateValues()
    {
        _bindableImage.SetImage(_imageProperty.Value);
    }
}
```

```csharp
public class CustomBindableElementsFactory : BindableElementsFactory
{
    public override IBindableElement Create(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
    {
        return bindableUiElement switch
        {
            BindableImage bindableImage => new BindableImageWrapper(bindableImage, objectProvider),

            _ => base.Create(bindableUiElement, objectProvider)
        };
    }
}
```

```csharp
public class ImageViewerViewModel : ViewModel
{
    private Texture2D _texture;

    public Texture2D Image
    {
        get => _texture;
        private set => Set(ref _texture, value);
    }
}
```

```csharp
public class ImageViewerView : DocumentView<ImageViewerViewModel>
{
    protected override IBindableElementsFactory GetBindableElementsFactory()
    {
        return new CustomBindableElementsFactory();
    }
}
```

```xml
<UXML>
    <BindableImage binding-image-path="Image" />
</UXML>
```

## :link: External Assets

### UniTask

To enable [async commands](#asynccommand--asynccommandt) support, you need to add the [UniTask](https://github.com/Cysharp/UniTask) package to your project.

In addition to async commands **UnityMvvmToolkit** provides extensions to make [USS transition's](https://docs.unity3d.com/Manual/UIE-Transitions.html) awaitable.

For example, your `VisualElement` has the following transitions.
```css
.panel--animation {
    transition-property: opacity, padding-bottom;
    transition-duration: 65ms, 150ms;
}
```

You can `await` these transitions using several methods.
```csharp
public async UniTask DeactivatePanel()
{
    try
    {
        panel.style.opacity = 0;
        panel.style.paddingBottom = 0;
        
        // Await for the 'opacity' || 'paddingBottom' to end or cancel.
        await panel.WaitForAnyTransitionEnd();
        
        // Await for the 'opacity' & 'paddingBottom' to end or cancel.
        await panel.WaitForAllTransitionsEnd();
        
        // Await 150ms.
        await panel.WaitForLongestTransitionEnd();

        // Await 65ms.
        await panel.WaitForTransitionEnd(0);
        
        // Await for the 'paddingBottom' to end or cancel.
        await panel.WaitForTransitionEnd(new StylePropertyName("padding-bottom"));
        
        // Await for the 'paddingBottom' to end or cancel.
        // Uses ReadOnlySpan to match property names to avoid memory allocation.
        await panel.WaitForTransitionEnd(nameof(panel.style.paddingBottom));
        
        // Await for the 'opacity' || 'paddingBottom' to end or cancel.
        // You can write your own transition predicates, just implement a 'ITransitionPredicate' interface.
        await panel.WaitForTransitionEnd(new TransitionAnyPredicate());
    }
    finally
    {
        panel.visible = false;
    }
}
```

> **Note:** All transition extensions have a `timeoutMs` parameter (default value is `2500ms`).

## :chart_with_upwards_trend: Benchmarks

The **UnityMvvmToolkit** uses delegates to get and set property values. This approach avoids boxing and unboxing for value types, and the performance improvements are really significant. In particular, this approach is ~65x faster than the one that uses standard `GetValue` and `SetValue` methods, and does not make any memory allocations at all.

<details><summary>Environment</summary>
<br />
<pre>
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19041.1165 (2004/May2020Update/20H1)
Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.301
  [Host]     : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT
  DefaultJob : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT
</pre>
</details>

#### Set & Get integer value

<pre>
|              Method |        Mean |     Error |    StdDev |  Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------- |------------:|----------:|----------:|-------:|-------:|------:|------:|----------:|
| DirectPropertyUsage |   0.4904 ns | 0.0364 ns | 0.0358 ns |   1.00 |      - |     - |     - |         - |
|    UnityMvvmToolkit |   3.4734 ns | 0.0925 ns | 0.0865 ns |   7.13 |      - |     - |     - |         - |
|          Reflection | 225.5382 ns | 4.4920 ns | 4.8063 ns | 463.38 | 0.0176 |     - |     - |     112 B |
</pre>

#### Complex binding

<pre>
|           Method |       Mean |    Error |   StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------- |-----------:|---------:|---------:|------:|-------:|------:|------:|----------:|
|   ManualApproach |   209.3 ns |  3.02 ns |  2.35 ns |  1.00 | 0.0458 |     - |     - |     288 B |
| UnityMvvmToolkit |   418.1 ns |  7.82 ns |  8.04 ns |  2.00 | 0.0458 |     - |     - |     288 B |
|       Reflection | 1,566.4 ns | 31.01 ns | 33.18 ns |  7.46 | 0.0725 |     - |     - |     464 B |
</pre>

## :bookmark_tabs: Contributing

You may contribute in several ways like creating new features, fixing bugs or improving documentation and examples.

### Discussions

Use [discussions](https://github.com/ChebanovDD/UnityMvvmToolkit/discussions) to have conversations and post answers without opening issues.

Discussions is a place to:
* Share ideas
* Ask questions
* Engage with other community members

### Report a bug

If you find a bug in the source code, please [create bug report](https://github.com/ChebanovDD/UnityMvvmToolkit/issues/new?assignees=ChebanovDD&labels=bug&template=bug_report.md&title=).

> Please browse [existing issues](https://github.com/ChebanovDD/UnityMvvmToolkit/issues) to see whether a bug has previously been reported.

### Request a feature

If you have an idea, or you're missing a capability that would make development easier, please [submit feature request](https://github.com/ChebanovDD/UnityMvvmToolkit/issues/new?assignees=ChebanovDD&labels=enhancement&template=feature_request.md&title=).

> If a similar feature request already exists, don't forget to leave a "+1" or add additional information, such as your thoughts and vision about the feature.

### Show your support

Give a :star: if this project helped you!

<a href="https://www.buymeacoffee.com/chebanovdd" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-orange.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>

## :balance_scale: License

Usage is provided under the [MIT License](LICENSE).
