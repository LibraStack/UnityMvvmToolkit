# UnityMvvmToolkit

A package that brings data-binding to your Unity project.

## :open_book: Table of Contents

- [About](#pencil-about)
  - [Restrictions](#restrictions)
  - [Samples](#samples)
- [Folder Structure](#cactus-folder-structure)
- [Installation](#gear-installation)
- [How To Use](#rocket-how-to-use)
  - [Add new icons set](#add-new-icons-set)
  - [Custom control]
- [External Assets](#external-assets)
  - [UniTask](#unitask)
- [Benchmarks](#benchmarks)
- [Contributing](#bookmark_tabs-contributing)
  - [Discussions](#discussions)
  - [Report a bug](#report-a-bug)
  - [Request a feature](#request-a-feature)
  - [Show your support](#show-your-support)
- [License](#balance_scale-license)

## :pencil: About

The **UnityMvvmToolkit** is designed to accelerate the development of MVVM applications in Unity. Use the samples as a starting point for understanding how to utilize the package.

<!--This repository contains initial samples for how to utilize the package.

It mostly designed for UI Toolkit but you can use it with UGUI as well.-->

It is built around the following principles:
- ...
- ...
- ...

### Restrictions

...

### Samples

<details open><summary><b>CounterView</b></summary>
<br />

```xml
<UXML>
    <BindableContentPage binding-theme-mode-path="ThemeMode" class="counter-screen">
        <VisualElement class="number-container">
            <BindableCountLabel binding-text-path="Count" class="count-label count-label--animation" />
        </VisualElement>
        <BindableThemeSwitcher binding-value-path="ThemeMode, Converter={ThemeModeToBoolConverter}" />
        <BindableCounterSlider increase-command="IncreaseCommand" decrease-command="DecreaseCommand" />
    </BindableContentPage>
</UXML>
```

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
        IncreaseCommand = new Command(IncreaseCount);
        DecreaseCommand = new Command(DecreaseCount);
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

    public ICommand IncreaseCommand { get; }
    public ICommand DecreaseCommand { get; }

    private void IncreaseCount()
    {
        Count++;
    }

    private void DecreaseCount()
    {
        Count--;
    }
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
    <td align="center">
      <video src="https://user-images.githubusercontent.com/28132516/187030099-a440bc89-4c28-44e3-9898-9894eac5bff4.mp4" alt="CounterSample" />
    </td>
    <td align="center">
      <video src="https://user-images.githubusercontent.com/28132516/187030102-2b02c663-31cb-4d63-a764-4be9484359f0.mp4" alt="CalculatorSample" />
    </td>
    <td align="center">
      <video src="https://user-images.githubusercontent.com/28132516/187030101-ad1f2123-59d5-4d1e-a9ca-ab983589e52f.mp4" alt="ToDoListSample" />
    </td>
  </tr>
</table>

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
    │       ├── Core      # Auto-generated
    │       ├── Common
    │       ├── External
    │       ├── UGUI
    │       └── UI        # UI Toolkit
    │
    ├── UnityMvvmToolkit.sln

## :gear: Installation

Dependencies:
- Unity UnityMvvmToolkit: [UniTask](https://openupm.com/packages/com.cysharp.unitask/)

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

  > **Note:** Dependencies must be installed before installing the package.
  
</details>

### [Releases Page](https://github.com/ChebanovDD/UnityMvvmToolkit/releases)

- ...
- ...
- ...

> **Note:** Dependencies must be installed before installing the packages.

## :rocket: How To Use

### Add new icons set

...

## :cherries: External Assets

### UniTask

#### Async commands

...
<!--For IAsyncCommand support, it is required to import com.demigiant.unitask from OpenUPM or-->

#### Transition async extensions

...

## :chart_with_upwards_trend: Benchmarks

...

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
