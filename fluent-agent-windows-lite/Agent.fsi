module Agent
type t
val make : TailWatcher.t list -> Fluentd.t -> t
val start : t -> unit
val stop  : t -> unit
