


<!-- PROJECT LOGO -->
<br />
<div align="left">
	<h1>Unity3D-Mario-Kart-Racing-Game</h1>
	</h1>

<!-- ABOUT THE PROJECT -->
## About The Project

Welcome to my open-source 3D kart racing game, created using C#, the .NET Framework, and Unity3D. This project remains popular with an active community on both GitHub and my YouTube Channel: Ishaan35.

I started this as a way to gain experience with C#, and learn what it is like to manage a large project. It took off when early videos gained traction, and the project has grown with positive community feedback, as I continued to work on it for over two years while gaining over 850 YouTube subscribers. 

I also decided to create this game as I couldn't find any open-source kart racing games that utilize similar physics to the very popular kart racing game, Mario Kart 8, and I wanted to change that with a challenge for myself.

I'm dedicated to pushing this project forward and making improvements. Whether you're a contributor, player, or just interested, you're part of this exciting journey. Let's build the future of this project together.

This game features 5 different tracks with detailed visuals to provide a great user experience.


Here is a video showing some gameplay of the project  [https://youtu.be/NfiscJR_nQY](https://youtu.be/NfiscJR_nQY), 
and the rest of the videos can be found on my [YouTube Channel](https://www.youtube.com/c/ishaan35).

![Game Image](https://github.com/Ishaan35/Unity3D-Mario-Kart-Racing-Game/blob/main/GameSnapshot.png?raw=true)

### Built With

* [![][C#]][C#-url]
* [![][Unity]][Unity-url]
* [![][.Net]][.Net-url]
<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- Status -->
## Status

This project is currently in development. I will be maintaining it for any major updates or features in Unity launches. The playable demo can be found here [here](https://drive.google.com/file/d/1YZ2OqAirBQlzf0WjMv1-zO6F597xrQoq/view?usp=sharing). 



## Usage

1. Download Unity version 2021.3.4f1 LTS or higher. LTS versions are always preferred for longer-term support of existing features used in the project.
2. Once downloaded, clone this GitHub repository, and open the root directory in Unity Hub as a project. I did not provide the library folder, so the first-time import may take a while.
3. Once in, you should be able to navigate to the scenes folder and view the different tracks. If you encounter errors or see unrendered pink meshes, try the following: 
	• Ensure the latest version of the Universal Render Pipeline and Shadergraph is installed as a package in the Unity Package Manager.
	• Make sure the version of Unity is LTS and at least version 2021.3.4f1.
	• Try downloading the full version of the project with the Library folder included [here](https://drive.google.com/file/d/16XfmmUwKhTlESbEOm70gqZQ0PQQe2iJf/view?usp=sharing)


<!-- ROADMAP -->
## Roadmap

- [x] Add multiple characters
- [ ] Add a main menu instead of a basic track selector


## Other Hurdles
• Anti-gravity was something that took me a while to fully figure out. I consider this a major achievement since it allows for a more diverse track selection that can be more twisty, potentially more difficult, and amusing with all the camera rotations. I implemented anti-gravity by switching to a mode where the gravitational force is applied to the direction that is opposite to the normal of the driving surface. I also made sure the kart smoothly rotates along the ground surface, and the camera rotation aligns with the player's rotation.

• Creating opponent players was also a challenge. I needed the right balance of versatility, randomness, and control over the movements of the players. Hence, I created a handful of possible path collections for each track. These paths would be nodes placed at intervals around the track. A large hitbox was placed around each node to determine if the computer player had passed a certain point on the track (which also provided a nice set-up for the ranking system during the race). Then I would randomly switch up the paths given to the computer players to give a more "random" feel to the movements of the computer players.

<!-- CONTACT -->
## Contact

Ishaan Patel  -  toishaanpatel@gmail.com

LinkedIn:  https://www.linkedin.com/in/ishaan35/

Personal Website: https://www.ishaanpatel.info/

Full Project Link with Library Folder Included: https://drive.google.com/file/d/16XfmmUwKhTlESbEOm70gqZQ0PQQe2iJf/view?usp=sharing

Playable Build Link: https://drive.google.com/file/d/1YZ2OqAirBQlzf0WjMv1-zO6F597xrQoq/view?usp=sharing








<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/othneildrew/Best-README-Template.svg?style=for-the-badge
[contributors-url]: https://github.com/othneildrew/Best-README-Template/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/othneildrew/Best-README-Template.svg?style=for-the-badge
[forks-url]: https://github.com/othneildrew/Best-README-Template/network/members
[stars-shield]: https://img.shields.io/github/stars/othneildrew/Best-README-Template.svg?style=for-the-badge
[stars-url]: https://github.com/othneildrew/Best-README-Template/stargazers
[issues-shield]: https://img.shields.io/github/issues/othneildrew/Best-README-Template.svg?style=for-the-badge
[issues-url]: https://github.com/othneildrew/Best-README-Template/issues
[license-shield]: https://img.shields.io/github/license/othneildrew/Best-README-Template.svg?style=for-the-badge
[license-url]: https://github.com/othneildrew/Best-README-Template/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/othneildrew
[product-screenshot]: images/screenshot.png


[Unity]: https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white
[Unity-url]: https://unity.com/
[C#]: https://img.shields.io/badge/C%23-9a48b1?style=for-the-badge&logo=c%20sharp&logoColor=white
[C#-url]: https://learn.microsoft.com/en-us/dotnet/csharp/
[.Net]: https://img.shields.io/badge/.NET%20Framework-512bd3?style=for-the-badge&logo=dotNet&logoColor=white
[.Net-url]: https://dotnet.microsoft.com/en-us/





FULL PROJECT DOWNLOAD: https://drive.google.com/file/d/16XfmmUwKhTlESbEOm70gqZQ0PQQe2iJf/view    (a few scene files were too large to fit on GitHub)


