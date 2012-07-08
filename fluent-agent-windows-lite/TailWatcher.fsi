module TailWatcher
type t
val make : string -> string -> string option -> t
val watch : t list -> t list
val lines : t -> string * string list
