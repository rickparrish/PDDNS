R&M PDDNS: A Promiscuous Dynamic DNS Client
===========================================

PDDNS was originally short for "Point Dynamic DNS", since it originally supported just Point DNS.  Then someone asked me to fix a bug in a really old client I wrote for DtDNS, so I decided rather than fixing the really old client that I don't even know if I have the source for, it would be better to just update PDDNS to support multiple clients.<br />
<br />
So as you can see from the title, PDDNS is now short for "Promiscuous Dynamic DNS" (based on the lesser known definition "consisting of parts, elements, or individuals of different kinds brought together without order.")<br />
<br />
Since I also had accounts for Dyn and NoIP, I figured I might as well add support for them in the first version as well.<br />
<br />
You'll also need to get <a href="https://github.com/rickparrish/RMLib">RMLib</a> and <a href="https://github.com/rickparrish/RMLibUI">RMLibUI</a> in order to compile this project.

Supported Providers
===================

Currently PDDNS supports <a href="http://dtdns.com">DtDNS</a>, <a href="http://dyn.com">Dyn</a>, <a href="http://noip.com">NoIP</a>, and <a href="http://pointhq.com">Point</a>.  I'm always open to supporting more, these just happened to be the ones I've used in the past and so had accounts ready to test with.

LICENSE
=======

    R&M PDDNS: A Promiscuous Dynamic DNS Client
    Copyright (C) 2013  Rick Parrish, R&M Software

    This file is part of PDDNS.

    PDDNS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    PDDNS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with PDDNS.  If not, see <http://www.gnu.org/licenses/>.
