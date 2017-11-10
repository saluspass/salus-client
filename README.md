# ipfs-pswmgr

![Release](https://img.shields.io/github/release/viperman1271/ipfs-pswmgr/all.svg)

![Demo Image](https://github.com/viperman1271/ipfs-pswmgr/blob/master/images/Password_Manager_Demo.jpg)

Decentralized zero knowldge password storage application using RSA 2048 encryption to encrypt and store password details, propagating the data files via [IPFS](https://ipfs.io/).

## Table of Contents

- [Prerequisites](#prerequisites)
- [Security Issues](#security-issues)
- [Dependencies](#dependencies)
- [Cryptography Notice](#cryptography-notice)
- [Building From Source](#building-from-source)
- [License](#license)

## Prerequisites

Either Visual Studio, the .NET framework (4.6.2), or an equivalent framework (such as Mono) must be installed prior to launching the application. In addition, IPFS must have already been installed on the client machine

## Security Issues

This application is still in heavy development, and not yet alpha quality. This means that the current implementation is __NOT SECURE__. This means that there may be problems. If you discover a security issue, please bring it to our attention right away!

If you find a vulnerability that may affect live deployments please report it on the GitHub page

## Dependencies
* [ipfs](https://www.ipfs.io/)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* [HTML Agility Pack](https://github.com/zzzprojects/html-agility-pack)
* [Extended WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit)
* [Semver](https://github.com/maxhauser/semver)

## Cryptography Notice

This distribution includes cryptographic software. The country in which you currently reside may have restrictions on the import, possession, use, and/or re-export to another country, of encryption software. __BEFORE__ using any encryption software, please check your country's laws, regulations and policies concerning the import, possession, or use, and re-export of encryption software, to see if this is permitted. See [http://www.wassenaar.org/](http://www.wassenaar.org/) for more information.

## Building From Source

Tested with Visual Studio 2017. Open ipfs-pswmgr.sln and build. Nuget packages should be restored automatically

## License
The main license for this project is the MIT License - [LICENSE](https://github.com/viperman1271/ipfs-pswmgr/blob/master/LICENSE)

Dependencies have their own licenses, which can be viewed at the following;
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
* [HTML Agility Pack](https://github.com/zzzprojects/html-agility-pack/blob/master/LICENSE)
* [Extended WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md)
* [Semver](https://github.com/maxhauser/semver/blob/master/License.txt)