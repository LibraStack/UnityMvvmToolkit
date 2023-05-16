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
  - [IBindingContext](#ibindingcontext)
  - [CanvasView](#canvasviewtbindingcontext)
  - [DocumentView](#documentviewtbindingcontext)
  - [Property](#propertyt--readonlypropertyt)
  - [Command](#command--commandt)
  - [AsyncCommand](#asynccommand--asynccommandt)
  - [PropertyValueConverter](#propertyvalueconvertertsourcetype-ttargettype)
  - [ParameterValueConverter](#parametervalueconverterttargettype)
- [Quick start](#watch-quick-start)
- [How To Use](#joystick-how-to-use)
  - [Data-binding](#data-binding)
  - [Create custom control](#create-custom-control)
  - [Source code generator](#source-code-generator)
- [External Assets](#link-external-assets)
  - [UniTask](#unitask)
- [Performance](#rocket-performance)
  - [Memory allocation](#memory-allocation)
- [Contributing](#bookmark_tabs-contributing)
  - [Discussions](#discussions)
  - [Report a bug](#report-a-bug)
  - [Request a feature](#request-a-feature)
  - [Show your support](#show-your-support)
- [License](#balance_scale-license)

## :pencil: About

The **UnityMvvmToolkit** allows you to use data binding to establish a connection between the app UI and the data it displays. This is a simple and consistent way to achieve clean separation of business logic from UI. Use the samples as a starting point for understanding how to utilize the package.

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
public class CounterViewModel : IBindingContext
{
    public CounterViewModel()
    {
        Count = new Property<int>();
        ThemeMode = new Property<ThemeMode>();

        IncrementCommand = new Command(IncrementCount);
        DecrementCommand = new Command(DecrementCount);
    }

    public IProperty<int> Count { get; }
    public IProperty<ThemeMode> ThemeMode { get; }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }

    private void IncrementCount() => Count.Value++;
    private void DecrementCount() => Count.Value--;
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

  If you want to set a target version, UnityMvvmToolkit uses the `v*.*.*` release tag, so you can specify a version like `#v1.0.0`. For example `https://github.com/ChebanovDD/UnityMvvmToolkit.git?path=src/UnityMvvmToolkit.UnityPackage/Assets/Plugins/UnityMvvmToolkit#v1.0.0`.
  
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
- [IBindingContext](#ibindingcontext)
- [CanvasView\<TBindingContext\>](#canvasviewtbindingcontext)
- [DocumentView\<TBindingContext\>](#documentviewtbindingcontext)
- [Property\<T\> & ReadOnlyProperty\<T\>](#propertyt--readonlypropertyt)
- [Command & Command\<T\>](#command--commandt)
- [AsyncCommand & AsyncCommand\<T\>](#asynccommand--asynccommandt)
- [PropertyValueConverter\<TSourceType, TTargetType\>](#propertyvalueconvertertsourcetype-ttargettype)
- [ParameterValueConverter\<TTargetType\>](#parametervalueconverterttargettype)
- [IProperty\<T\> & IReadOnlyProperty\<T\>](#propertyt--readonlypropertyt)
- [ICommand & ICommand\<T\>](#command--commandt)
- [IAsyncCommand & IAsyncCommand\<T\>](#asynccommand--asynccommandt)
- [IPropertyValueConverter\<TSourceType, TTargetType\>](#propertyvalueconvertertsourcetype-ttargettype)
- [IParameterValueConverter\<TTargetType\>](#parametervalueconverterttargettype)

### IBindingContext

The `IBindingContext` is a base interface for ViewModels. It is a marker for Views that the class contains observable properties to bind to.

> **Note:** In case your ViewModel doesn't have a parameterless constructor, you need to override the `GetBindingContext` method in the View.

#### Simple property

Here's an example of how to implement notification support to a custom property.

```csharp
public class CounterViewModel : IBindingContext
{
    public CounterViewModel()
    {
        Count = new Property<int>();
    }

    public IProperty<int> Count { get; }
}
```

#### Wrapping a non-observable model

A common scenario, for instance, when working with collection items, is to create a wrapping "bindable" item model that relays properties of the collection item model, and raises the property value changed notifications when needed.

```csharp
public class ItemViewModel : IBindingContext
{
    [Observable(nameof(Name))]
    private readonly IProperty<string> _name = new Property<string>();

    public string Name
    {
        get => _name.Value;
        set => _name.Value = value;
    }
}
```

The `ItemViewModel` can be serialized and deserialized without any issues.

### CanvasView\<TBindingContext\>

The `CanvasView<TBindingContext>` is a base class for `uGUI` views.

Key functionality:
- Provides a base implementation for `Canvas` based view
- Automatically searches for bindable UI elements on the `Canvas`
- Allows to override the base viewmodel instance creation
- Allows to define [property](#propertyvalueconvertertsourcetype-ttargettype) & [parameter](#parametervalueconverterttargettype) value converters

```csharp
public class CounterView : CanvasView<CounterViewModel>
{
    // Override the base viewmodel instance creation.
    // Required in case the viewmodel doesn't have a parameterless constructor.
    protected override CounterViewModel GetBindingContext()
    {
        return _appContext.Resolve<CounterViewModel>();
    }

    // Define 'property' & 'parameter' value converters.
    protected override IValueConverter[] GetValueConverters()
    {
        return _appContext.Resolve<IValueConverter[]>();
    }
}
```

### DocumentView\<TBindingContext\>

The `DocumentView<TBindingContext>` is a base class for `UI Toolkit` views.

Key functionality:
- Provides a base implementation for `UI Document` based view
- Automatically searches for bindable UI elements on the `UI Document`
- Allows to override the base viewmodel instance creation
- Allows to define [property](#propertyvalueconvertertsourcetype-ttargettype) & [parameter](#parametervalueconverterttargettype) value converters

```csharp
public class CounterView : DocumentView<CounterViewModel>
{
    // Override the base viewmodel instance creation.
    // Required in case the viewmodel doesn't have a parameterless constructor.
    protected override CounterViewModel GetBindingContext()
    {
        return _appContext.Resolve<CounterViewModel>();
    }

    // Define 'property' & 'parameter' value converters.
    protected override IValueConverter[] GetValueConverters()
    {
        return _appContext.Resolve<IValueConverter[]>();
    }
}
```

### Property\<T\> & ReadOnlyProperty\<T\>

The `Property<T>` and `ReadOnlyProperty<T>` provide a way to bind properties between a ViewModel and UI elements.

Key functionality:
- Provide a base implementation of the `IBaseProperty` interface
- Implement the `IProperty<T>` & `IReadOnlyProperty<T>` interface, which exposes a `ValueChanged` event

The following shows how to set up a simple observable property:
- [Simple property](#simple-property)
- [Wrapping a non-observable model](#wrapping-a-non-observable-model)

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

public class CounterViewModel : IBindingContext
{
    public CounterViewModel()
    {
        Count = new Property<int>();
        
        IncrementCommand = new Command(IncrementCount);
    }

    public IProperty<int> Count { get; }

    public ICommand IncrementCommand { get; }

    private void IncrementCount() => Count.Value++;
}
```

And the relative UI could then be.

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <uitk:BindableLabel binding-text-path="Count" />
    <uitk:BindableButton command="IncrementCommand" />
</ui:UXML>
```

The `BindableButton` binds to the `ICommand` in the viewmodel, which wraps the private `IncrementCount` method. The `BindableLabel` displays the value of the `Count` property and is updated every time the property value changes.

> **Note:** You need to define `IntToStrConverter` to convert int to string. See the [PropertyValueConverter](#propertyvalueconvertertsourcetype-ttargettype) section for more information.

### AsyncCommand & AsyncCommand\<T\>

The `AsyncCommand` and `AsyncCommand<T>` are `ICommand` implementations that extend the functionalities offered by `Command`, with support for asynchronous operations.

Key functionality:
- Extend the functionalities of the synchronous commands included in the package, with support for UniTask-returning delegates
- Can wrap asynchronous functions with a `CancellationToken` parameter to support cancelation, and they expose a `DisableOnExecution` property, as well as a `Cancel` method
- Implement the `IAsyncCommand` & `IAsyncCommand<T>` interfaces, which allows to replace a command with a custom implementation, if needed

Let's say we want to download an image from the web and display it as soon as it downloads.

```csharp
public class ImageViewerViewModel : IBindingContext
{
    [Observable(nameof(Image))]
    private readonly IProperty<Texture2D> _image;
    private readonly IImageDownloader _imageDownloader;

    public ImageViewerViewModel(IImageDownloader imageDownloader)
    {
        _image = new Property<Texture2D>();
        _imageDownloader = imageDownloader;
        
        DownloadImageCommand = new AsyncCommand(DownloadImageAsync);
    }

    public Texture2D Image => _image.Value;

    public IAsyncCommand DownloadImageCommand { get; }

    private async UniTask DownloadImageAsync(CancellationToken cancellationToken)
    {
        _image.Value = await _imageDownloader.DownloadRandomImageAsync(cancellationToken);
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

> **Note:** The `BindableImage` is a custom control from the [create custom control](#create-custom-control) section.

To disable the `BindableButton` while an async operation is running, simply set the `DisableOnExecution` property of the `AsyncCommand` to `true`.

```csharp
public class ImageViewerViewModel : IBindingContext
{
    public ImageViewerViewModel(IImageDownloader imageDownloader)
    {
        ...
        DownloadImageCommand = new AsyncCommand(DownloadImageAsync) { DisableOnExecution = true };
    }
}
```

To allow the same async command to be invoked concurrently multiple times, set the `AllowConcurrency` property of the `AsyncCommand` to `true`.

```csharp
public class MainViewModel : IBindingContext
{
    public MainViewModel()
    {
        RunConcurrentlyCommand = new AsyncCommand(RunConcurrentlyAsync) { AllowConcurrency = true };
    }
}
```

If you want to create an async command that supports cancellation, use the `WithCancellation` extension method.

```csharp
public class MyViewModel : IBindingContext
{
    public MyViewModel()
    {
        MyAsyncCommand = new AsyncCommand(DoSomethingAsync).WithCancellation();
        CancelCommand = new Command(Cancel);
    }

    public IAsyncCommand MyAsyncCommand { get; }
    public ICommand CancelCommand { get; }
    
    private async UniTask DoSomethingAsync(CancellationToken cancellationToken)
    {
        ...
    }
    
    private void Cancel()
    {
        // If the underlying command is not running, this method will perform no action.
        MyAsyncCommand.Cancel();
    }
}
```

If a command supports cancellation and the `AllowConcurrency` property is set to `true`, all running commands will be canceled.

> **Note:** You need to import the [UniTask](https://github.com/Cysharp/UniTask) package in order to use async commands.

### PropertyValueConverter\<TSourceType, TTargetType\>

Property value converter provides a way to apply custom logic to a property binding.

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
    <!--Full expression-->
    <MyBindableElement binding-value-path="ThemeMode, Converter={ThemeModeToBoolConverter}" />
    <!--Short expression-->
    <MyBindableElement binding-value-path="ThemeMode, ThemeModeToBoolConverter" />
    <!--Minimal expression - the first appropriate converter will be used-->
    <MyBindableElement binding-value-path="ThemeMode" />
</UXML>
```

### ParameterValueConverter\<TTargetType\>

Parameter value converter allows to convert a command parameter.

Built-in parameter value converters:
- ParameterToIntConverter
- ParameterToFloatConverter

By default, the converter is not needed if your command has a `string` parameter type.

```csharp
public class MyViewModel : IBindingContext
{
    public MyViewModel()
    {
        PrintParameterCommand = new Command<string>(PrintParameter);
    }

    public ICommand<string> PrintParameterCommand { get; }

    private void PrintParameter(string parameter)
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
    public override int Convert(string parameter)
    {
        return int.Parse(parameter);
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
public class MyViewModel : IBindingContext
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
    <!--Full expression-->
    <BindableButton command="PrintIntParameterCommand, Parameter={5}, Converter={ParameterToIntConverter}" />
    <!--Short expression-->
    <BindableButton command="PrintIntParameterCommand, 5, ParameterToIntConverter" />
    <!--Minimal expression - the first appropriate converter will be used-->
    <BindableButton command="PrintIntParameterCommand, 5" />
</UXML>
```

## :watch: Quick start

Once the `UnityMVVMToolkit` is installed, create a class `MyFirstViewModel` that implements the `IBindingContext` interface.

```csharp
using UnityMvvmToolkit.Core;

public class MyFirstViewModel : IBindingContext
{
    public MyFirstViewModel()
    {
        Text = new ReadOnlyProperty<string>("Hello World");
    }

    public IReadOnlyProperty<string> Text { get; }
}
```

#### UI Toolkit

The next step is to create a class `MyFirstDocumentView` that inherits the `DocumentView<TBindingContext>` class.

```csharp
using UnityMvvmToolkit.UITK;

public class MyFirstDocumentView : DocumentView<MyFirstViewModel>
{
}
```

Then create a file `MyFirstView.uxml`, add a `BindableLabel` control and set the `binding-text-path` to `Text`.

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <uitk:BindableLabel binding-text-path="Text" />
</ui:UXML>
```

Finally, add `UI Document` to the scene, set the `MyFirstView.uxml` as a `Source Asset` and add the `MyFirstDocumentView` component to it.

<details><summary>UI Document Inspector</summary>
<br />

![ui-document-inspector](https://user-images.githubusercontent.com/28132516/187613060-e20a139d-72fc-4088-b8d5-f9a01f5afa5b.png)

</details>

#### Unity UI (uGUI)

For the `uGUI` do the following. Create a class `MyFirstCanvasView` that inherits the `CanvasView<TBindingContext>` class.

```csharp
using UnityMvvmToolkit.UGUI;

public class MyFirstCanvasView : CanvasView<MyFirstViewModel>
{
}
```

Then add a `Canvas` to the scene, and add the `MyFirstCanvasView` component to it.

<details><summary>Canvas Inspector</summary>
<br />

![canvas-inspector](https://user-images.githubusercontent.com/28132516/187613633-2c61c82e-ac25-4319-8e8d-1954eb4be197.png)

</details>

Finally, add a `Text - TextMeshPro` UI element to the canvas, add the `BindableLabel` component to it and set the `BindingTextPath` to `Text`.

<details><summary>Canvas Text Inspector</summary>
<br />

![canvas-text-inspector](https://user-images.githubusercontent.com/28132516/187614103-ad42d000-b3b7-4265-96a6-f6d4db6e8978.png)

</details>

## :joystick: How To Use

### Data-binding

The package contains a set of standard bindable UI elements out of the box.

The included UI elements are:
- [BindableLabel](#bindablelabel)
- [BindableTextField](#bindabletextfield)
- [BindableButton](#bindablebutton)
- [BindableListView](#bindablelistview)
- [BindableScrollView](#bindablescrollview)

> **Note:** The `BindableListView` & `BindableScrollView` are provided for `UI Toolkit` only.

#### BindableLabel

The `BindableLabel` element uses the `OneWay` binding by default.

```csharp
public class LabelViewModel : IBindingContext
{
    public LabelViewModel()
    {
        IntValue = new Property<int>(55);
        StrValue = new Property<string>("69");
    }

    public IReadOnlyProperty<int> IntValue { get; }
    public IReadOnlyProperty<string> StrValue { get; }
}

public class LabelView : DocumentView<LabelViewModel>
{
    protected override IValueConverter[] GetValueConverters()
    {
        return new IValueConverter[] { new IntToStrConverter() };
    }
}
```

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <uitk:BindableLabel binding-text-path="StrValue" />
    <uitk:BindableLabel binding-text-path="IntValue" />
</ui:UXML>
```

#### BindableTextField

The `BindableTextField` element uses the `TwoWay` binding by default.

```csharp
public class TextFieldViewModel : IBindingContext
{
    public TextFieldViewModel()
    {
        TextValue = new Property<string>();
    }

    public IProperty<string> TextValue { get; }
}
```

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <uitk:BindableTextField binding-text-path="TextValue" />
</ui:UXML>
```

#### BindableButton

The `BindableButton` can be bound to the following commands:
- [Command & Command\<T\>](#command--commandt)
- [AsyncCommand & AsyncCommand\<T\>](#asynccommand--asynccommandt)
- [AsyncLazyCommand & AsyncLazyCommand\<T\>](#asynclazycommand--asynclazycommandt)

To pass a parameter to the viewmodel, see the [ParameterValueConverter](#parametervalueconverterttargettype) section.

#### BindableListView

The `BindableListView` control is the most efficient way to create lists. It uses virtualization and creates VisualElements only for visible items. Use the `binding-items-source-path` of the `BindableListView` to bind to an `ObservableCollection`.

The following example demonstrates how to bind to a collection of users with `BindableListView`.

Create a main `UI Document` named `UsersView.uxml` with the following content.

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <uitk:BindableListView binding-items-source-path="Users" />
</ui:UXML>
```

Create a `UI Document` named `UserItemView.uxml` for the individual items in the list.

```xml
<ui:UXML xmlns:uitk="UnityMvvmToolkit.UITK.BindableUIElements" ...>
    <uitk:BindableLabel binding-text-path="Name" />
</ui:UXML>
```

Create a `UserItemViewModel` class that implements `ICollectionItem` to store user data.

```csharp
public class UserItemViewModel : ICollectionItem
{
    [Observable(nameof(Name))] 
    private readonly IProperty<string> _name = new Property<string>();

    public UserItemViewModel()
    {
        Id = Guid.NewGuid().GetHashCode();
    }

    public int Id { get; }

    public string Name
    {
        get => _name.Value;
        set => _name.Value = value;
    }
}
```

Create a `UserListView` that inherits the `BindableListViewWrapper<TItemBindingContext>` abstract class.

```csharp
public class UserListView : BindableListView<UserItemViewModel>
{
    public new class UxmlFactory : UxmlFactory<UserListView, UxmlTraits> {}
}
```

Create a `UsersViewModel`.

```csharp
public class UsersViewModel : IBindableContext
{
    public UsersViewModel()
    {
        var users = new ObservableCollection<UserItemViewModel>
        {
            new() { Name = "User 1" },
            new() { Name = "User 2" },
            new() { Name = "User 3" },
        };

        Users = new ReadOnlyProperty<ObservableCollection<UserItemViewModel>>(users);
    }

    public IReadOnlyProperty<ObservableCollection<UserItemViewModel>> Users { get; }
}
```

Create a `UsersView` with the following content.

```csharp
public class UsersView : DocumentView<UsersViewModel>
{
    [SerializeField] private VisualTreeAsset _userItemViewAsset;

    protected override IReadOnlyDictionary<Type, object> GetCollectionItemTemplates()
    {
        return new Dictionary<Type, object>
        {
            { typeof(UserItemViewModel), _userItemViewAsset }
        };
    }
}
```

#### BindableScrollView

The `BindableScrollView` has the same binding logic as the `BindableListView`. It does not use virtualization and creates VisualElements for all items regardless of visibility.

### Create custom control

Let's create a `BindableImage` UI element.

First of all, create a base `Image` class.

```csharp
public class Image : VisualElement
{
    public void SetImage(Texture2D image)
    {
        style.backgroundImage = new StyleBackground(image);
    }

    public new class UxmlFactory : UxmlFactory<Image, UxmlTraits> {}
}
```

Then create a `BindableImage` class and implement the data binding logic.

```csharp
public class BindableImage : Image, IBindableElement
{
    private PropertyBindingData _imagePathBindingData;
    private IReadOnlyProperty<Texture2D> _imageProperty;

    public string BindingImagePath { get; private set; }

    public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
    {
        _imagePathBindingData ??= BindingImagePath.ToPropertyBindingData();

        _imageProperty = objectProvider.RentReadOnlyProperty<Texture2D>(context, _imagePathBindingData);
        _imageProperty.ValueChanged += OnImagePropertyValueChanged;

        SetImage(_imageProperty.Value);
    }

    public void ResetBindingContext(IObjectProvider objectProvider)
    {
        if (_imageProperty == null)
        {
            return;
        }

        _imageProperty.ValueChanged -= OnImagePropertyValueChanged;

        objectProvider.ReturnReadOnlyProperty(_imageProperty);

        _imageProperty = null;

        SetImage(null);
    }

    private void OnImagePropertyValueChanged(object sender, Texture2D newImage)
    {
        SetImage(newImage);
    }

    public new class UxmlFactory : UxmlFactory<BindableImage, UxmlTraits> { }

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

Now you can use the new UI element as following.

```csharp
public class ImageViewerViewModel : IBindingContext
{
    public ImageItemViewModel(Texture2D image)
    {
        Image = new ReadOnlyProperty<Texture2D>(image);
    }

    public IReadOnlyProperty<Texture2D> Image { get; }
}
```

```xml
<UXML>
    <BindableImage binding-image-path="Image" />
</UXML>
```

### Source code generator
  
The best way to speed up the creation of custom `VisualElement` is to use source code generators. With this powerful tool, you can achieve the same great results with minimal boilerplate code and focus on what really matters: programming!
  
Let's create the `BindableImage` control, but this time using source code generators.

For a visual element without bindings, we will use a [UnityUxmlGenerator](https://github.com/LibraStack/UnityUxmlGenerator).

```csharp
[UxmlElement]
public partial class Image : VisualElement
{
    public void SetImage(Texture2D image)
    {
        style.backgroundImage = new StyleBackground(image);
    }
}
```
  
<details><summary><b>Generated code</b></summary>
<br />

`Image.UxmlFactory.g.cs`
  
```csharp
partial class Image
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityUxmlGenerator", "1.0.0.0")]
    public new class UxmlFactory : global::UnityEngine.UIElements.UxmlFactory<Image, UxmlTraits> 
    {
    }
}
```
  
</details>
  
For a bindable visual element, we will use a [UnityMvvmToolkit.Generator](https://github.com/LibraStack/UnityMvvmToolkit.Generator).

```csharp
[BindableElement]
public partial class BindableImage : Image
{
    [BindableProperty]
    private IReadOnlyProperty<Texture2D> _imageProperty;

    partial void AfterSetBindingContext(IBindingContext context, IObjectProvider objectProvider)
    {
        SetImage(_imageProperty?.Value);
    }

    partial void AfterResetBindingContext(IObjectProvider objectProvider)
    {
        SetImage(null);
    }

    partial void OnImagePropertyValueChanged([CanBeNull] Texture2D value)
    {
        SetImage(value);
    }
}
```
  
<details><summary><b>Generated code</b></summary>
<br />

`BindableImage.Bindings.g.cs`
  
```csharp
partial class BindableImage : global::UnityMvvmToolkit.Core.Interfaces.IBindableElement
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    private global::UnityMvvmToolkit.Core.PropertyBindingData? _imageBindingData;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public void SetBindingContext(global::UnityMvvmToolkit.Core.Interfaces.IBindingContext context,
        global::UnityMvvmToolkit.Core.Interfaces.IObjectProvider objectProvider)
    {
        BeforeSetBindingContext(context, objectProvider);

        if (string.IsNullOrWhiteSpace(BindingImagePath) == false)
        {
            _imageBindingData ??=
                global::UnityMvvmToolkit.Core.Extensions.StringExtensions.ToPropertyBindingData(BindingImagePath!);
            _imageProperty = objectProvider.RentReadOnlyProperty<global::UnityEngine.Texture2D>(context, _imageBindingData!);
            _imageProperty!.ValueChanged += OnImagePropertyValueChanged;
        }

        AfterSetBindingContext(context, objectProvider);
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public void ResetBindingContext(global::UnityMvvmToolkit.Core.Interfaces.IObjectProvider objectProvider)
    {
        BeforeResetBindingContext(objectProvider);

        if (_imageProperty != null)
        {
            _imageProperty!.ValueChanged -= OnImagePropertyValueChanged;
            objectProvider.ReturnReadOnlyProperty(_imageProperty);
            _imageProperty = null;
        }

        AfterResetBindingContext(objectProvider);
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private void OnImagePropertyValueChanged(object sender, global::UnityEngine.Texture2D value)
    {
        OnImagePropertyValueChanged(value);
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    partial void BeforeSetBindingContext(global::UnityMvvmToolkit.Core.Interfaces.IBindingContext context,
        global::UnityMvvmToolkit.Core.Interfaces.IObjectProvider objectProvider);

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    partial void AfterSetBindingContext(global::UnityMvvmToolkit.Core.Interfaces.IBindingContext context,
        global::UnityMvvmToolkit.Core.Interfaces.IObjectProvider objectProvider);

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    partial void BeforeResetBindingContext(global::UnityMvvmToolkit.Core.Interfaces.IObjectProvider objectProvider);

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    partial void AfterResetBindingContext(global::UnityMvvmToolkit.Core.Interfaces.IObjectProvider objectProvider);

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    partial void OnImagePropertyValueChanged(global::UnityEngine.Texture2D value);
}
```

`BindableImage.Uxml.g.cs`

```csharp
partial class BindableImage
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private string BindingImagePath { get; set; }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    public new class UxmlFactory : global::UnityEngine.UIElements.UxmlFactory<BindableImage, UxmlTraits>
    {
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
    public new class UxmlTraits : global::BindableUIElements.Image.UxmlTraits
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
        private readonly global::UnityEngine.UIElements.UxmlStringAttributeDescription _bindingImagePath = new() 
            { name = "binding-image-path", defaultValue = "" };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("UnityMvvmToolkit.Generator", "1.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override void Init(global::UnityEngine.UIElements.VisualElement visualElement, 
            global::UnityEngine.UIElements.IUxmlAttributes bag, 
            global::UnityEngine.UIElements.CreationContext context)
        {
            base.Init(visualElement, bag, context);

            var control = (BindableImage) visualElement;
            control.BindingImagePath = _bindingImagePath.GetValueFromBag(bag, context);
        }
    }
}
```
  
</details>

As you can see, using [UnityUxmlGenerator](https://github.com/LibraStack/UnityUxmlGenerator) and [UnityMvvmToolkit.Generator](https://github.com/LibraStack/UnityMvvmToolkit.Generator) we can achieve the same results but with just a few lines of code.

> **Note:** The [UnityMvvmToolkit.Generator](https://github.com/LibraStack/UnityMvvmToolkit.Generator) is available exclusively for my [patrons](https://patreon.com/DimaChebanov).
  
## :link: External Assets

### UniTask

To enable [async commands](#asynccommand--asynccommandt) support, you need to add the [UniTask](https://github.com/Cysharp/UniTask) package to your project.

In addition to async commands **UnityMvvmToolkit** provides extensions to make [USS transition](https://docs.unity3d.com/Manual/UIE-Transitions.html)'s awaitable.

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

## :rocket: Performance

### Memory allocation

The **UnityMvvmToolkit** uses object pools under the hood and reuses created objects. You can warm up certain objects in advance to avoid allocations during execution time.

```csharp
public abstract class BaseView<TBindingContext> : DocumentView<TBindingContext>
        where TBindingContext : class, IBindingContext
{
    protected override IObjectProvider GetObjectProvider()
    {
        return new BindingContextObjectProvider(new IValueConverter[] { new IntToStrConverter() })
            // Finds and warmups all classes from calling assembly that implement IBindingContext.
            .WarmupAssemblyViewModels()
            // Finds and warmups all classes from certain assembly that implement IBindingContext.
            .WarmupAssemblyViewModels(Assembly.GetExecutingAssembly())
            // Warmups a certain class.
            .WarmupViewModel<CounterViewModel>()
            // Warmups a certain class.
            .WarmupViewModel(typeof(CounterViewModel))
            // Creates 5 instances to rent 'IProperty<string>' without any allocations.
            .WarmupValueConverter<IntToStrConverter>(5);
    }
}
```

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
