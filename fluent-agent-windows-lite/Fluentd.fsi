module Fluentd
type t
val make : string -> int -> t
val autoReconnect : t -> t
val emit : string -> string list -> t -> t
