// Copyright 2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Serilog.Crestron.Sinks.CrestronConsole.Themes
{
    internal static class AnsiEscapeSequence
    {
        public const string UNTHEMED = "";
        public const string RESET = "\x1b[0m";
        public const string BOLD = "\x1b[1m";

        public const string BLACK = "\x1b[30m";
        public const string RED = "\x1b[31m";
        public const string GREEN = "\x1b[32m";
        public const string YELLOW = "\x1b[33m";
        public const string BLUE = "\x1b[34m";
        public const string MAGENTA = "\x1b[35m";
        public const string CYAN = "\x1b[36m";
        public const string WHITE = "\x1b[37m";

        public const string BRIGHT_BLACK = "\x1b[30;1m";
        public const string BRIGHT_RED = "\x1b[31;1m";
        public const string BRIGHT_GREEN = "\x1b[32;1m";
        public const string BRIGHT_YELLOW = "\x1b[33;1m";
        public const string BRIGHT_BLUE = "\x1b[34;1m";
        public const string BRIGHT_MAGENTA = "\x1b[35;1m";
        public const string BRIGHT_CYAN = "\x1b[36;1m";
        public const string BRIGHT_WHITE = "\x1b[37;1m";
    }
}