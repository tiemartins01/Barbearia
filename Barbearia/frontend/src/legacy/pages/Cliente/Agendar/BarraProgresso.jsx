export default function BarraProgresso({ etapa }) {

    return (

        <div className="flex gap-2">

            {[1, 2, 3, 4].map((item) => (

                <div
                    key={item}
                    className={`
                        h-1.5
                        flex-1
                        rounded-full
                        transition-all
                        duration-300
                        ${etapa >= item
                            ? "bg-primary"
                            : "bg-zinc-700"}
                    `}
                />

            ))}

        </div>

    );

}